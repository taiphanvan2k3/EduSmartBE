import { ICommandHandler, CommandHandler } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { BadRequestException, Logger, NotFoundException } from '@nestjs/common';
import { Repository } from 'typeorm';
import { v4 as uuidv4 } from 'uuid';
import { CourseAchievementTemplate } from '../../../shared/database/entities/course-achievement-template.entity';
import { CreateCourseTemplateCommand } from '../commands/create-course-template.command';
import { UpdateCourseTemplateCommand } from '../commands/update-course-template.command';
import { DeleteCourseTemplateCommand } from '../commands/delete-course-template.command';

function convertToDto(row: CourseAchievementTemplate) {
  return {
    id: row.id,
    courseId: row.courseId,
    templateId: row.achievementTemplateId,
    templateURL: row.achievementTemplate
      ? row.achievementTemplate.templateUrl
      : null,
    courseNameTextStyle: row.courseNameTextStyle,
    studentNameTextStyle: row.studentNameTextStyle,
    dateTextStyle: row.dateTextStyle,
    teacherNameTextStyle: row.teacherNameTextStyle,
    isDefault: row.isDefault,
  };
}

@CommandHandler(CreateCourseTemplateCommand)
export class CreateCourseTemplateHandler implements ICommandHandler<CreateCourseTemplateCommand> {
  private readonly logger = new Logger(CreateCourseTemplateHandler.name);

  constructor(
    @InjectRepository(CourseAchievementTemplate)
    private readonly repo: Repository<CourseAchievementTemplate>,
  ) {}

  async execute(command: CreateCourseTemplateCommand) {
    const { dto } = command;
    this.logger.log(`Creating template for course: ${dto.courseId}`);
    try {
      const existing = await this.repo.findOne({
        where: { courseId: dto.courseId },
      });
      if (existing) {
        throw new BadRequestException('Course has already had a template');
      }

      const id = uuidv4();
      const newTemplate = this.repo.create({
        id,
        courseId: dto.courseId,
        achievementTemplateId: dto.templateId,
        courseNameTextStyle: dto.courseNameTextStyle,
        studentNameTextStyle: dto.studentNameTextStyle,
        dateTextStyle: dto.dateTextStyle,
        teacherNameTextStyle: dto.teacherNameTextStyle,
        isDefault: dto.isDefault,
      });

      await this.repo.save(newTemplate);
      return id;
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error(
        `Error creating template for course ${dto.courseId}`,
        err.stack,
      );
      throw error;
    }
  }
}

@CommandHandler(UpdateCourseTemplateCommand)
export class UpdateCourseTemplateHandler implements ICommandHandler<UpdateCourseTemplateCommand> {
  private readonly logger = new Logger(UpdateCourseTemplateHandler.name);

  constructor(
    @InjectRepository(CourseAchievementTemplate)
    private readonly repo: Repository<CourseAchievementTemplate>,
  ) {}

  async execute(command: UpdateCourseTemplateCommand) {
    const { id, dto } = command;
    this.logger.log(`Updating template: ${id}`);
    try {
      const template = await this.repo.findOne({ where: { id } });
      if (!template) {
        throw new NotFoundException('Course template not found');
      }

      template.achievementTemplateId = dto.templateId;
      template.isDefault = dto.isDefault;
      template.studentNameTextStyle = dto.studentNameTextStyle;
      template.courseNameTextStyle = dto.courseNameTextStyle;
      template.dateTextStyle = dto.dateTextStyle;
      template.teacherNameTextStyle = dto.teacherNameTextStyle;

      const updated = await this.repo.save(template);

      const result = await this.repo.findOne({
        where: { id: updated.id },
        relations: { achievementTemplate: true },
      });
      return convertToDto(result!);
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error(`Error updating template ${id}`, err.stack);
      throw error;
    }
  }
}

@CommandHandler(DeleteCourseTemplateCommand)
export class DeleteCourseTemplateHandler implements ICommandHandler<DeleteCourseTemplateCommand> {
  private readonly logger = new Logger(DeleteCourseTemplateHandler.name);

  constructor(
    @InjectRepository(CourseAchievementTemplate)
    private readonly repo: Repository<CourseAchievementTemplate>,
  ) {}

  async execute(command: DeleteCourseTemplateCommand) {
    const { id } = command;
    this.logger.log(`Deleting template: ${id}`);
    try {
      const result = await this.repo.delete(id);
      if (result.affected === 0) {
        throw new NotFoundException('Course template not found');
      }
      return id;
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error(`Error deleting template ${id}`, err.stack);
      throw error;
    }
  }
}
