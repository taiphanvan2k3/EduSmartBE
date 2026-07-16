import { IQueryHandler, QueryHandler, QueryBus } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { NotFoundException } from '@nestjs/common';
import { Repository } from 'typeorm';
import { GetAchievementExportStatusQuery } from '../queries/get-achievement-export-status.query';
import { StudentAchievement } from '../../../shared/database/entities/student-achievement.entity';
import { GrpcCourseService } from '../../../shared/grpc/grpc-course.service';
import { GetDefaultCourseTemplateQuery } from '../../course-template/queries/get-default-course-template.query';
import { TextStyle } from '../services/canvas.service';

interface CourseTemplateResult {
  templateId: number;
  templateURL: string | null;
  studentNameTextStyle: TextStyle;
  courseNameTextStyle: TextStyle;
  dateTextStyle: TextStyle;
  teacherNameTextStyle: TextStyle;
}

@QueryHandler(GetAchievementExportStatusQuery)
export class GetAchievementExportStatusHandler implements IQueryHandler<GetAchievementExportStatusQuery> {
  constructor(
    @InjectRepository(StudentAchievement)
    private readonly studentAchievementRepository: Repository<StudentAchievement>,
    private readonly grpcCourseService: GrpcCourseService,
    private readonly queryBus: QueryBus,
  ) {}

  async execute(query: GetAchievementExportStatusQuery) {
    const { courseId, studentId } = query;

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
      return {
        canExport: false,
        achievement: null,
        achievementExportInfo: null,
      };
    }

    const exported = await this.studentAchievementRepository.findOne({
      where: { courseId, studentId },
    });

    if (exported) {
      return {
        canExport: true,
        exportedAchievement: {
          achievementURL: exported.achievementUrl,
          createdAt: exported.createdAt,
        },
        achievementExportInfo: null,
      };
    }

    const defaultTemplate = (await this.queryBus.execute(
      new GetDefaultCourseTemplateQuery(courseId),
    )) as unknown as CourseTemplateResult | null;

    if (!defaultTemplate) {
      throw new NotFoundException(
        'Your teacher has not created any template for this course',
      );
    }

    return {
      canExport: true,
      exportedAchievement: null,
      achievementExportInfo: {
        templateId: defaultTemplate.templateId,
        templateURL: defaultTemplate.templateURL,
        studentNameTextStyle: defaultTemplate.studentNameTextStyle,
        courseNameTextStyle: defaultTemplate.courseNameTextStyle,
        dateTextStyle: defaultTemplate.dateTextStyle,
        teacherNameTextStyle: defaultTemplate.teacherNameTextStyle,
        courseName: checkStatus.courseName,
        studentName: checkStatus.studentName,
        teacherName: checkStatus.teacherName,
      },
    };
  }
}
