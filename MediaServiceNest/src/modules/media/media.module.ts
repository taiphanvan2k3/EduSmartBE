import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CqrsModule } from '@nestjs/cqrs';
import { MediaController } from './media.controller';
import { StorageInfo } from '../../shared/database/entities/storage-info.entity';
import { CloudinaryModule } from '../../shared/cloudinary/cloudinary.module';
import { GetUserStorageInfoHandler } from './handlers/get-user-storage-info.handler';
import { UploadFileHandler } from './handlers/upload-file.handler';

@Module({
  imports: [
    CqrsModule,
    TypeOrmModule.forFeature([StorageInfo]),
    CloudinaryModule,
  ],
  controllers: [MediaController],
  providers: [GetUserStorageInfoHandler, UploadFileHandler],
})
export class MediaModule {}
