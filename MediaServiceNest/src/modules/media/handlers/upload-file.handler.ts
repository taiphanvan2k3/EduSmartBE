import { ICommandHandler, CommandHandler } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { BadRequestException, Logger, NotFoundException } from '@nestjs/common';
import { Repository } from 'typeorm';
import { StorageInfo } from '../../../shared/database/entities/storage-info.entity';
import { CloudinaryService } from '../../../shared/cloudinary/cloudinary.service';
import { UploadFileCommand } from '../commands/upload-file.command';

const ALLOWED_RAW_MIMES = [
  'application/pdf',
  'application/msword',
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
  'application/vnd.ms-excel',
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  'text/plain',
];

@CommandHandler(UploadFileCommand)
export class UploadFileHandler implements ICommandHandler<UploadFileCommand> {
  private readonly logger = new Logger(UploadFileHandler.name);

  constructor(
    @InjectRepository(StorageInfo)
    private readonly storageInfoRepository: Repository<StorageInfo>,
    private readonly cloudinaryService: CloudinaryService,
  ) {}

  async execute(command: UploadFileCommand) {
    const { userId, file } = command;

    const fileSizeInKB = file.size / 1024;

    // Check storage capacity
    const storageInfo = await this.storageInfoRepository.findOne({
      where: { userId },
    });
    if (!storageInfo) {
      throw new NotFoundException('User not found');
    }

    const currentUsedStorage = Number(storageInfo.usedStorage);
    const maximumStorage = Number(storageInfo.maximumStorage);
    if (currentUsedStorage + fileSizeInKB > maximumStorage) {
      throw new BadRequestException(
        'Dung lượng đã vượt quá giới hạn, vui lòng nâng cấp để tiếp tục sử dụng.',
      );
    }

    // Determine resource type
    const resourceType: 'auto' | 'raw' | 'image' | 'video' = ALLOWED_RAW_MIMES.includes(
      file.mimetype,
    )
      ? 'raw'
      : 'auto';

    // Upload to Cloudinary
    const result = await this.cloudinaryService.uploadCloudinary(
      file.buffer,
      'files',
      resourceType,
      file.originalname,
    );

    // Update storage usage in DB
    storageInfo.usedStorage = Math.round(currentUsedStorage + fileSizeInKB);
    await this.storageInfoRepository.save(storageInfo);

    this.logger.log(`File uploaded for user ${userId}: ${result.secure_url}`);

    return {
      message: 'File uploaded successfully',
      data: {
        fileUrl: result.secure_url,
        filePublicId: result.public_id,
      },
    };
  }
}
