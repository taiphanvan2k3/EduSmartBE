import { Injectable, Logger } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { v2 as cloudinary, UploadApiResponse } from 'cloudinary';

@Injectable()
export class CloudinaryService {
  private readonly logger = new Logger(CloudinaryService.name);

  constructor(private readonly configService: ConfigService) {
    cloudinary.config({
      cloud_name: this.configService.get<string>('CLOUDINARY_CLOUD_NAME'),
      api_key: this.configService.get<string>('CLOUDINARY_API_KEY'),
      api_secret: this.configService.get<string>('CLOUDINARY_API_SECRET'),
    });
    this.logger.log('Cloudinary configured successfully.');
  }

  /**
   * Upload file from buffer to Cloudinary
   */
  async uploadCloudinary(
    fileBuffer: Buffer,
    folderName: string,
    resourceType: 'auto' | 'raw' | 'image' | 'video' = 'auto',
    fileName = '',
  ): Promise<UploadApiResponse> {
    this.logger.log(`Uploading file buffer to folder: ${folderName}...`);

    const uploadOptions: any = {
      folder: folderName,
      resource_type: resourceType,
    };

    if (resourceType === 'raw' && fileName) {
      uploadOptions.public_id = fileName;
    }

    return new Promise((resolve, reject) => {
      cloudinary.uploader
        .upload_stream(uploadOptions, (error, result) => {
          if (error) {
            this.logger.error(
              'Error uploading file to Cloudinary',
              error.message,
            );
            reject(error);
          } else {
            this.logger.log('File uploaded successfully to Cloudinary');
            resolve(result as UploadApiResponse);
          }
        })
        .end(fileBuffer);
    });
  }

  /**
   * Upload file from a local path to Cloudinary
   */
  async uploadCloudinaryFromFilePath(
    filePath: string,
    folderName: string,
  ): Promise<UploadApiResponse> {
    this.logger.log(
      `Uploading file from path ${filePath} to folder: ${folderName}...`,
    );

    const uploadOptions = {
      folder: folderName,
    };

    return new Promise((resolve, reject) => {
      cloudinary.uploader.upload(filePath, uploadOptions, (error, result) => {
        if (error) {
          this.logger.error(
            'Error uploading file to Cloudinary',
            error.message,
          );
          reject(error);
        } else {
          this.logger.log('File uploaded successfully from local path');
          resolve(result as UploadApiResponse);
        }
      });
    });
  }

  /**
   * Delete resource from Cloudinary by public ID
   */
  async deleteCloudinary(publicId: string): Promise<any> {
    this.logger.log(
      `Deleting file with public ID: ${publicId} from Cloudinary...`,
    );

    return new Promise((resolve, reject) => {
      cloudinary.uploader.destroy(publicId, (error, result) => {
        if (error) {
          this.logger.error(`Error deleting file ${publicId}`, error.message);
          reject(error);
        } else {
          resolve(result);
        }
      });
    });
  }
}
