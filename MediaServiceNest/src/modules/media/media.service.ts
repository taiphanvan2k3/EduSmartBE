import { Injectable, NotFoundException, Logger } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { StorageInfo } from '../../shared/database/entities/storage-info.entity';

@Injectable()
export class MediaService {
  private readonly logger = new Logger(MediaService.name);

  constructor(
    @InjectRepository(StorageInfo)
    private readonly storageInfoRepository: Repository<StorageInfo>,
  ) {}

  /**
   * Get storage usage details of a user
   */
  async getUserStorageInfo(userId: number): Promise<StorageInfo[]> {
    try {
      this.logger.log(`Fetching storage info for user: ${userId}`);
      const storageInfo = await this.storageInfoRepository.find({
        where: { userId },
      });
      return storageInfo;
    } catch (error: any) {
      this.logger.error(`Error fetching storage info for user ${userId}`, error.stack);
      throw new Error('Error fetching data from StorageInfo');
    }
  }

  /**
   * Update storage usage of a user
   */
  async updateStorageInfo(userId: number, extraStorage: number): Promise<{ message: string }> {
    try {
      this.logger.log(`Updating storage info for user: ${userId} with extraStorage: ${extraStorage} KB`);
      const storageInfo = await this.storageInfoRepository.findOne({
        where: { userId },
      });

      if (storageInfo) {
        const extraStorageBytes = Math.round(extraStorage);
        const currentUsedStorage = Number(storageInfo.usedStorage);
        const newUserStorage = currentUsedStorage + extraStorageBytes;

        storageInfo.usedStorage = newUserStorage;
        await this.storageInfoRepository.save(storageInfo);
        return { message: 'User storage updated successfully.' };
      } else {
        throw new NotFoundException('User not found');
      }
    } catch (error: any) {
      this.logger.error(`Error updating StorageInfo for user ${userId}`, error.stack);
      throw new Error('Error updating StorageInfo: ' + error.message);
    }
  }

  /**
   * Check if a user has enough storage left
   */
  async isEnoughStorage(userId: number, extraStorage: number): Promise<boolean> {
    try {
      this.logger.log(`Checking storage capacity for user: ${userId} with extraStorage: ${extraStorage} KB`);
      const storageInfo = await this.storageInfoRepository.findOne({
        where: { userId },
      });

      if (storageInfo) {
        const currentUsedStorage = Number(storageInfo.usedStorage);
        const maximumStorage = Number(storageInfo.maximumStorage);
        const newUserStorage = currentUsedStorage + extraStorage;

        return newUserStorage <= maximumStorage;
      } else {
        throw new NotFoundException('User not found');
      }
    } catch (error: any) {
      this.logger.error(`Error checking storage for user ${userId}`, error.stack);
      throw new Error('Error checking storage');
    }
  }
}
