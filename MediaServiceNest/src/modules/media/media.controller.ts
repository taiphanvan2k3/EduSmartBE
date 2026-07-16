import {
  Controller,
  Get,
  Post,
  UseGuards,
  UseInterceptors,
  UploadedFile,
  BadRequestException,
  HttpCode,
  HttpStatus,
} from '@nestjs/common';
import { FileInterceptor } from '@nestjs/platform-express';
import {
  ApiTags,
  ApiBearerAuth,
  ApiOperation,
  ApiConsumes,
  ApiBody,
  ApiResponse,
} from '@nestjs/swagger';
import { MediaService } from './media.service';
import { CloudinaryService } from '../../shared/cloudinary/cloudinary.service';
import { JwtAuthGuard } from '../../common/guards/jwt-auth.guard';
import { CurrentUser } from '../../common/decorators/current-user.decorator';

export interface UserPayload {
  userId: number;
  username?: string;
  email?: string;
  fullName?: string;
  role: string;
}

@ApiTags('Medias')
@ApiBearerAuth('BearerAuth')
@UseGuards(JwtAuthGuard)
@Controller('api/media')
export class MediaController {
  constructor(
    private readonly mediaService: MediaService,
    private readonly cloudinaryService: CloudinaryService,
  ) {}

  @Get()
  @ApiOperation({ summary: "Get user's media storage info" })
  @ApiResponse({ status: 200, description: 'Return storage details' })
  async getMedia(@CurrentUser() user: UserPayload) {
    const userId = user.userId;
    return this.mediaService.getUserStorageInfo(userId);
  }

  @Post('upload-file')
  @HttpCode(HttpStatus.OK)
  @UseInterceptors(FileInterceptor('file'))
  @ApiConsumes('multipart/form-data')
  @ApiOperation({ summary: 'Upload a file' })
  @ApiBody({
    schema: {
      type: 'object',
      properties: {
        file: {
          type: 'string',
          format: 'binary',
          description: 'The file to upload',
        },
      },
    },
  })
  @ApiResponse({ status: 200, description: 'File uploaded successfully' })
  @ApiResponse({
    status: 400,
    description: 'Storage capacity exceeded or invalid file',
  })
  async uploadFile(
    @CurrentUser() user: UserPayload,
    @UploadedFile() file: Express.Multer.File,
  ) {
    const userId = user.userId;

    if (!file) {
      throw new BadRequestException('No file provided!');
    }

    const fileSizeInKB = file.size / 1024; // Convert bytes to KB

    // Check if there is enough storage
    const hasEnoughStorage = await this.mediaService.isEnoughStorage(
      userId,
      fileSizeInKB,
    );
    if (!hasEnoughStorage) {
      throw new BadRequestException(
        'Dung lượng đã vượt quá giới hạn, vui lòng nâng cấp để tiếp tục sử dụng.',
      );
    }

    // Determine resource type
    let resourceType: 'auto' | 'raw' | 'image' | 'video' = 'auto';
    const allowedRawMimes = [
      'application/pdf',
      'application/msword',
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
      'application/vnd.ms-excel',
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'text/plain',
    ];

    if (allowedRawMimes.includes(file.mimetype)) {
      resourceType = 'raw';
    }

    // Upload to Cloudinary
    const result = await this.cloudinaryService.uploadCloudinary(
      file.buffer,
      'files',
      resourceType,
      file.originalname,
    );

    const fileUrl = result.secure_url;
    const filePublicId = result.public_id;

    // Update user's storage usage in database
    await this.mediaService.updateStorageInfo(userId, fileSizeInKB);

    return {
      message: 'File uploaded successfully',
      data: {
        fileUrl,
        filePublicId,
      },
    };
  }
}
