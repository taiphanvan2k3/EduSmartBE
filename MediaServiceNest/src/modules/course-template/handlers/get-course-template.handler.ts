import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { Logger, NotFoundException } from '@nestjs/common';
import { Repository } from 'typeorm';
import { GetDefaultCourseTemplateQuery } from '../queries/get-default-course-template.query';
import { GetCourseTemplateByIdQuery } from '../queries/get-course-template-by-id.query';
import { CourseAchievementTemplate } from '../../../shared/database/entities/course-achievement-template.entity';

function convertToDto(row: CourseAchievementTemplate) {
  return {
    id: row.id,
    courseId: row.courseId,
    templateId: row.achievementTemplateId,
    templateURL: row.achievementTemplate ? row.achievementTemplate.templateUrl : null,
    courseNameTextStyle: row.courseNameTextStyle,
    studentNameTextStyle: row.studentNameTextStyle,
    dateTextStyle: row.dateTextStyle,
    teacherNameTextStyle: row.teacherNameTextStyle,
    isDefault: row.isDefault,
  };
}

@QueryHandler(GetDefaultCourseTemplateQuery)
export class GetDefaultCourseTemplateHandler implements IQueryHandler<GetDefaultCourseTemplateQuery> {
  private readonly logger = new Logger(GetDefaultCourseTemplateHandler.name);

  constructor(
    @InjectRepository(CourseAchievementTemplate)
    private readonly repo: Repository<CourseAchievementTemplate>,
  ) {}

  async execute(query: GetDefaultCourseTemplateQuery) {
    const { courseId } = query;
    this.logger.log(`Fetching default template for course: ${courseId}`);
    try {
      const row = await this.repo.findOne({
        where: { courseId, isDefault: true },
        relations: { achievementTemplate: true },
      });
      return row ? convertToDto(row) : null;
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error(`Error fetching default template for course ${courseId}`, err.stack);
      throw error;
    }
  }
}

@QueryHandler(GetCourseTemplateByIdQuery)
export class GetCourseTemplateByIdHandler implements IQueryHandler<GetCourseTemplateByIdQuery> {
  private readonly logger = new Logger(GetCourseTemplateByIdHandler.name);

  constructor(
    @InjectRepository(CourseAchievementTemplate)
    private readonly repo: Repository<CourseAchievementTemplate>,
  ) {}

  async execute(query: GetCourseTemplateByIdQuery) {
    const { id } = query;
    this.logger.log(`Fetching template by ID: ${id}`);
    try {
      const row = await this.repo.findOne({
        where: { id },
        relations: { achievementTemplate: true },
      });
      if (!row) {
        throw new NotFoundException('Course template not found');
      }
      return convertToDto(row);
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error(`Error fetching template ${id}`, err.stack);
      throw error;
    }
  }
}
