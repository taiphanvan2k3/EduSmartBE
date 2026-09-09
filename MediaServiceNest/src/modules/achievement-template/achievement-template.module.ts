import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CqrsModule } from '@nestjs/cqrs';
import { AchievementTemplateController } from './achievement-template.controller';
import { AchievementTemplate } from '../../shared/database/entities/achievement-template.entity';
import { GetAllTemplatesHandler } from './handlers/get-all-templates.handler';

@Module({
  imports: [CqrsModule, TypeOrmModule.forFeature([AchievementTemplate])],
  controllers: [AchievementTemplateController],
  providers: [GetAllTemplatesHandler],
})
export class AchievementTemplateModule {}
