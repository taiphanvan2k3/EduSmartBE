import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AchievementTemplateController } from './achievement-template.controller';
import { AchievementTemplateService } from './achievement-template.service';
import { AchievementTemplate } from '../../shared/database/entities/achievement-template.entity';

@Module({
  imports: [TypeOrmModule.forFeature([AchievementTemplate])],
  controllers: [AchievementTemplateController],
  providers: [AchievementTemplateService],
  exports: [AchievementTemplateService],
})
export class AchievementTemplateModule {}
