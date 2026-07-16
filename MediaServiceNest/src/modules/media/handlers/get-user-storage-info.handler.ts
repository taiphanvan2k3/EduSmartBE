import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { Logger } from '@nestjs/common';
import { Repository } from 'typeorm';
import { GetUserStorageInfoQuery } from '../queries/get-user-storage-info.query';
import { StorageInfo } from '../../../shared/database/entities/storage-info.entity';

@QueryHandler(GetUserStorageInfoQuery)
export class GetUserStorageInfoHandler implements IQueryHandler<GetUserStorageInfoQuery> {
  private readonly logger = new Logger(GetUserStorageInfoHandler.name);

  constructor(
    @InjectRepository(StorageInfo)
    private readonly storageInfoRepository: Repository<StorageInfo>,
  ) {}

  async execute(query: GetUserStorageInfoQuery): Promise<StorageInfo[]> {
    const { userId } = query;
    try {
      this.logger.log(`Fetching storage info for user: ${userId}`);
      return await this.storageInfoRepository.find({ where: { userId } });
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error(
        `Error fetching storage info for user ${userId}`,
        err.stack,
      );
      throw new Error('Error fetching data from StorageInfo');
    }
  }
}
