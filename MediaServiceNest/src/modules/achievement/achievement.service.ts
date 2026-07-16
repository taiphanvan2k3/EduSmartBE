import {
  Injectable,
  NotFoundException,
  BadRequestException,
  Logger,
} from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { v4 as uuidv4 } from 'uuid';
import { StudentAchievement } from '../../shared/database/entities/student-achievement.entity';
import { CourseAchievementTemplate } from '../../shared/database/entities/course-achievement-template.entity';
import { GrpcCourseService } from '../../shared/grpc/grpc-course.service';
import { CloudinaryService } from '../../shared/cloudinary/cloudinary.service';
import { CanvasService, TextStyle } from './services/canvas.service';
import { CourseTemplateService } from '../course-template/course-template.service';
import { saveAchievementLocally } from '../../common/utils/file.utils';

@Injectable()
export class AchievementService {
  private readonly logger = new Logger(AchievementService.name);

  constructor(
    @InjectRepository(StudentAchievement)
    private readonly studentAchievementRepository: Repository<StudentAchievement>,
    @InjectRepository(CourseAchievementTemplate)
    private readonly courseTemplateRepository: Repository<CourseAchievementTemplate>,
    private readonly grpcCourseService: GrpcCourseService,
    private readonly cloudinaryService: CloudinaryService,
    private readonly canvasService: CanvasService,
    private readonly courseTemplateService: CourseTemplateService,
  ) {}

  /**
   * Get the achievement export status of a student in a course
   */
  async getAchievementExportStatus(courseId: string, studentId: number) {
    this.logger.log(
      `Checking export status for course: ${courseId}, student: ${studentId}`,
    );

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

    // If not exported yet, fetch the default course template details
    const defaultTemplate =
      await this.courseTemplateService.getDefaultCourseTemplateInCourse(
        courseId,
      );
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
        studentNameTextStyle: defaultTemplate.studentNameTextStyle as TextStyle,
        courseNameTextStyle: defaultTemplate.courseNameTextStyle as TextStyle,
        dateTextStyle: defaultTemplate.dateTextStyle as TextStyle,
        teacherNameTextStyle: defaultTemplate.teacherNameTextStyle as TextStyle,
        courseName: checkStatus.courseName,
        studentName: checkStatus.studentName,
        teacherName: checkStatus.teacherName,
      },
    };
  }

  /**
   * Generate certificate for student who completed the course
   */
  async generateAchievement(courseId: string, studentId: number) {
    this.logger.log(
      `Generating achievement image for course: ${courseId}, student: ${studentId}`,
    );

    const isExported = await this.checkIsExportedAchievement(
      courseId,
      studentId,
    );
    if (isExported) {
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

    const defaultTemplate =
      await this.courseTemplateService.getDefaultCourseTemplateInCourse(
        courseId,
      );
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
      defaultTemplate.studentNameTextStyle as TextStyle,
      defaultTemplate.courseNameTextStyle as TextStyle,
      defaultTemplate.dateTextStyle as TextStyle,
      defaultTemplate.teacherNameTextStyle as TextStyle,
    );

    return {
      courseId,
      localPath: localResult.localPath,
      localPathInPublic: localResult.localPathInPublic,
      createdAt: new Date().toISOString(),
    };
  }

  /**
   * Save uploaded certificate (exported by Web frontend) to DB and upload to Cloudinary
   */
  async saveExportedStudentAchievementFromWeb(
    courseId: string,
    studentId: number,
    studentName: string,
    achievementFile: Express.Multer.File,
  ) {
    this.logger.log(
      `Saving certificate uploaded from web for course: ${courseId}, student: ${studentId}`,
    );

    const isExported = await this.checkIsExportedAchievement(
      courseId,
      studentId,
    );
    if (isExported) {
      throw new BadRequestException(
        'Achievement has already been exported in this course',
      );
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

    const localResult = saveAchievementLocally(achievementFile, studentName);

    // Save to Cloudinary & DB in background/asynchronously (using setImmediate like original Express app)
    setImmediate(() => {
      void (async () => {
        try {
          await this.saveExportedStudentAchievement(
            courseId,
            studentId,
            localResult.localPath,
          );
        } catch (error: unknown) {
          const err = error as Error;
          this.logger.error(
            `Background upload task failed for student ${studentId}`,
            err.stack,
          );
        }
      })();
    });

    return {
      courseId,
      localPath: localResult.localPath,
      localPathInPublic: localResult.localPathInPublic,
      createdAt: new Date().toISOString(),
    };
  }

  /**
   * Perform actual upload to Cloudinary and insert to StudentAchievements table
   */
  async saveExportedStudentAchievement(
    courseId: string,
    studentId: number,
    localPath: string,
  ) {
    this.logger.log(
      `Background uploading to Cloudinary from path: ${localPath}`,
    );

    const uploadRes = await this.cloudinaryService.uploadCloudinaryFromFilePath(
      localPath,
      'achievements',
    );

    const id = uuidv4();
    const studentAchievement = this.studentAchievementRepository.create({
      id,
      courseId,
      studentId,
      achievementUrl: uploadRes.secure_url,
    });

    await this.studentAchievementRepository.save(studentAchievement);
    this.logger.log(
      `Achievement saved successfully to database for course: ${courseId}, student: ${studentId}`,
    );
  }

  private async checkIsExportedAchievement(
    courseId: string,
    studentId: number,
  ): Promise<boolean> {
    const count = await this.studentAchievementRepository.count({
      where: { courseId, studentId },
    });
    return count > 0;
  }
}
