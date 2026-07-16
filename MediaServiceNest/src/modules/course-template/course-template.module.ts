import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CqrsModule } from '@nestjs/cqrs';
import { CourseTemplateController } from './course-template.controller';
import { CourseAchievementTemplate } from '../../shared/database/entities/course-achievement-template.entity';
// Query handlers
import {
  GetDefaultCourseTemplateHandler,
  GetCourseTemplateByIdHandler,
} from './handlers/get-course-template.handler';
// Command handlers
import {
  CreateCourseTemplateHandler,
  UpdateCourseTemplateHandler,
  DeleteCourseTemplateHandler,
} from './handlers/course-template-commands.handler';

@Module({
  imports: [CqrsModule, TypeOrmModule.forFeature([CourseAchievementTemplate])],
  controllers: [CourseTemplateController],
  providers: [
    GetDefaultCourseTemplateHandler,
    GetCourseTemplateByIdHandler,
    CreateCourseTemplateHandler,
    UpdateCourseTemplateHandler,
    DeleteCourseTemplateHandler,
  ],
})
export class CourseTemplateModule {}
