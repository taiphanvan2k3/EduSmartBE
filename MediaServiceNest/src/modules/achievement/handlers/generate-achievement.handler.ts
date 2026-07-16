import { ICommandHandler, CommandHandler, QueryBus } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { BadRequestException, Logger, NotFoundException } from '@nestjs/common';
import { Repository } from 'typeorm';
import { GenerateAchievementCommand } from '../commands/generate-achievement.command';
import { StudentAchievement } from '../../../shared/database/entities/student-achievement.entity';
import { GrpcCourseService } from '../../../shared/grpc/grpc-course.service';
import { CanvasService, TextStyle } from '../services/canvas.service';
import { GetDefaultCourseTemplateQuery } from '../../course-template/queries/get-default-course-template.query';

interface CourseTemplateResult {
  templateId: number;
  templateURL: string | null;
  studentNameTextStyle: TextStyle;
  courseNameTextStyle: TextStyle;
  dateTextStyle: TextStyle;
  teacherNameTextStyle: TextStyle;
}

@CommandHandler(GenerateAchievementCommand)
export class GenerateAchievementHandler implements ICommandHandler<GenerateAchievementCommand> {
  private readonly logger = new Logger(GenerateAchievementHandler.name);

  constructor(
    @InjectRepository(StudentAchievement)
    private readonly studentAchievementRepository: Repository<StudentAchievement>,
    private readonly grpcCourseService: GrpcCourseService,
    private readonly canvasService: CanvasService,
    private readonly queryBus: QueryBus,
  ) {}

  async execute(command: GenerateAchievementCommand) {
    const { courseId, studentId } = command;

    this.logger.log(
      `Generating achievement image for course: ${courseId}, student: ${studentId}`,
    );

    const count = await this.studentAchievementRepository.count({
      where: { courseId, studentId },
    });
    if (count > 0) {
      throw new BadRequestException('Achievement has already been exported');
    }

    const checkStatus =
      await this.grpcCourseService.checkStudentCompletedCourse(
        courseId,
        studentId,
      );
    if (!checkStatus.isSuccess) {
      throw new Error(
        checkStatus.message || 'gRPC CheckStudentCompletedCourse call failed',
      );
    }

    if (!checkStatus.isCompleted) {
      throw new BadRequestException('Student has not completed the course');
    }

    const defaultTemplate = (await this.queryBus.execute(
      new GetDefaultCourseTemplateQuery(courseId),
    )) as unknown as CourseTemplateResult | null;

    if (!defaultTemplate) {
      throw new NotFoundException(
        'Your teacher has not created any template for this course',
      );
    }

    const localResult = await this.canvasService.createAchievement(
      defaultTemplate.templateId,
      checkStatus.studentName,
      checkStatus.courseName,
      checkStatus.teacherName,
      defaultTemplate.studentNameTextStyle,
      defaultTemplate.courseNameTextStyle,
      defaultTemplate.dateTextStyle,
      defaultTemplate.teacherNameTextStyle,
    );

    return {
      courseId,
      localPath: localResult.localPath,
      localPathInPublic: localResult.localPathInPublic,
      createdAt: new Date().toISOString(),
    };
  }
}
