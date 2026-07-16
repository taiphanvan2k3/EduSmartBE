import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CourseTemplateController } from './course-template.controller';
import { CourseTemplateService } from './course-template.service';
import { CourseAchievementTemplate } from '../../shared/database/entities/course-achievement-template.entity';

@Module({
  imports: [TypeOrmModule.forFeature([CourseAchievementTemplate])],
  controllers: [CourseTemplateController],
  providers: [CourseTemplateService],
  exports: [CourseTemplateService],
})
export class CourseTemplateModule {}
