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
import { CommandBus, QueryBus } from '@nestjs/cqrs';
import { FileInterceptor } from '@nestjs/platform-express';
import {
  ApiTags,
  ApiBearerAuth,
  ApiOperation,
  ApiConsumes,
  ApiBody,
  ApiResponse,
} from '@nestjs/swagger';
import { JwtAuthGuard } from '../../common/guards/jwt-auth.guard';
import { CurrentUser } from '../../common/decorators/current-user.decorator';
import { GetUserStorageInfoQuery } from './queries/get-user-storage-info.query';
import { UploadFileCommand } from './commands/upload-file.command';

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
    private readonly commandBus: CommandBus,
    private readonly queryBus: QueryBus,
  ) {}

  @Get()
  @ApiOperation({ summary: "Get user's media storage info" })
  @ApiResponse({ status: 200, description: 'Return storage details' })
  async getMedia(@CurrentUser() user: UserPayload): Promise<unknown> {
    return this.queryBus.execute(new GetUserStorageInfoQuery(user.userId));
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
  ): Promise<unknown> {
    if (!file) {
      throw new BadRequestException('No file provided!');
    }
    return this.commandBus.execute(new UploadFileCommand(user.userId, file));
  }
}
