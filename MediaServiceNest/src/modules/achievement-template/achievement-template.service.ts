import { Injectable, Logger } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { AchievementTemplate } from '../../shared/database/entities/achievement-template.entity';

@Injectable()
export class AchievementTemplateService {
  private readonly logger = new Logger(AchievementTemplateService.name);

  constructor(
    @InjectRepository(AchievementTemplate)
    private readonly templateRepository: Repository<AchievementTemplate>,
  ) {}

  async getAllTemplates() {
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
