import { Injectable, NotFoundException, BadRequestException, Logger } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { v4 as uuidv4 } from 'uuid';
import { CourseAchievementTemplate } from '../../shared/database/entities/course-achievement-template.entity';
import { CreateCourseTemplateDto } from './dto/create-course-template.dto';
import { UpdateCourseTemplateDto } from './dto/update-course-template.dto';

@Injectable()
export class CourseTemplateService {
  private readonly logger = new Logger(CourseTemplateService.name);

  constructor(
    @InjectRepository(CourseAchievementTemplate)
    private readonly courseTemplateRepository: Repository<CourseAchievementTemplate>,
  ) {}

  /**
   * Get the default template for a course
   */
  async getDefaultCourseTemplateInCourse(courseId: string) {
    this.logger.log(`Fetching default template for course: ${courseId}`);
    try {
      const row = await this.courseTemplateRepository.findOne({
        where: { courseId, isDefault: true },
        relations: { achievementTemplate: true },
      });
      return row ? this.convertCourseTemplateToDto(row) : null;
    } catch (error: any) {
      this.logger.error(`Error fetching default template for course ${courseId}`, error.stack);
      throw error;
    }
  }

  /**
   * Get course template by ID
   */
  async getCourseTemplateById(id: string) {
    this.logger.log(`Fetching template by ID: ${id}`);
    try {
      const row = await this.courseTemplateRepository.findOne({
        where: { id },
        relations: { achievementTemplate: true },
      });
      if (!row) {
        throw new NotFoundException('Course template not found');
      }
      return this.convertCourseTemplateToDto(row);
    } catch (error: any) {
      this.logger.error(`Error fetching template ${id}`, error.stack);
      throw error;
    }
  }

  /**
   * Create template for a course
   */
  async createTemplateForCourse(dto: CreateCourseTemplateDto) {
    this.logger.log(`Creating template for course: ${dto.courseId}`);
    try {
      // Check if course already has a template
      const existing = await this.courseTemplateRepository.findOne({
        where: { courseId: dto.courseId },
      });
      if (existing) {
        throw new BadRequestException('Course has already had a template');
      }

      const id = uuidv4();
      const newTemplate = this.courseTemplateRepository.create({
        id,
        courseId: dto.courseId,
        achievementTemplateId: dto.templateId,
        courseNameTextStyle: dto.courseNameTextStyle,
        studentNameTextStyle: dto.studentNameTextStyle,
        dateTextStyle: dto.dateTextStyle,
        teacherNameTextStyle: dto.teacherNameTextStyle,
        isDefault: dto.isDefault,
      });

      await this.courseTemplateRepository.save(newTemplate);
      return id;
    } catch (error: any) {
      this.logger.error(`Error creating template for course ${dto.courseId}`, error.stack);
      throw error;
    }
  }

  /**
   * Update template for a course
   */
  async updateTemplateForCourse(id: string, dto: UpdateCourseTemplateDto) {
    this.logger.log(`Updating template: ${id}`);
    try {
      const template = await this.courseTemplateRepository.findOne({
        where: { id },
      });
      if (!template) {
        throw new NotFoundException('Course template not found');
      }

      template.achievementTemplateId = dto.templateId;
      template.isDefault = dto.isDefault;
      template.studentNameTextStyle = dto.studentNameTextStyle;
      template.courseNameTextStyle = dto.courseNameTextStyle;
      template.dateTextStyle = dto.dateTextStyle;
      template.teacherNameTextStyle = dto.teacherNameTextStyle;

      const updated = await this.courseTemplateRepository.save(template);
      
      // Fetch updated with relations
      const result = await this.courseTemplateRepository.findOne({
        where: { id: updated.id },
        relations: { achievementTemplate: true },
      });
      return this.convertCourseTemplateToDto(result!);
    } catch (error: any) {
      this.logger.error(`Error updating template ${id}`, error.stack);
      throw error;
    }
  }

  /**
   * Delete course template by ID
   */
  async deleteCourseTemplate(id: string) {
    this.logger.log(`Deleting template: ${id}`);
    try {
      const result = await this.courseTemplateRepository.delete(id);
      if (result.affected === 0) {
        throw new NotFoundException('Course template not found');
      }
      return id;
    } catch (error: any) {
      this.logger.error(`Error deleting template ${id}`, error.stack);
      throw error;
    }
  }

  /**
   * Helper to convert entity to DTO structure
   */
  private convertCourseTemplateToDto(row: CourseAchievementTemplate) {
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
}
