import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { Logger } from '@nestjs/common';
import { Repository } from 'typeorm';
import { GetAllTemplatesQuery } from '../queries/get-all-templates.query';
import { AchievementTemplate } from '../../../shared/database/entities/achievement-template.entity';

@QueryHandler(GetAllTemplatesQuery)
export class GetAllTemplatesHandler implements IQueryHandler<GetAllTemplatesQuery> {
  private readonly logger = new Logger(GetAllTemplatesHandler.name);

  constructor(
    @InjectRepository(AchievementTemplate)
    private readonly templateRepository: Repository<AchievementTemplate>,
  ) {}

  async execute(_: GetAllTemplatesQuery) {
    this.logger.log('Fetching all achievement templates...');
    try {
      const rows = await this.templateRepository.find();
      return rows.map((row) => ({
        id: row.id,
        name: row.name,
        thumbnailURL: row.thumbnailUrl,
        templateURL: row.templateUrl,
      }));
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error('Error fetching achievement templates', err.stack);
      throw error;
    }
  }
}
