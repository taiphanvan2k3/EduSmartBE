import { ICommandHandler, CommandHandler } from '@nestjs/cqrs';
import { InjectRepository } from '@nestjs/typeorm';
import { BadRequestException, Logger } from '@nestjs/common';
import { Repository } from 'typeorm';
import { v4 as uuidv4 } from 'uuid';
import { SaveAchievementFromWebCommand } from '../commands/save-achievement-from-web.command';
import { StudentAchievement } from '@shared/database/entities/student-achievement.entity';
import { GrpcCourseService } from '@shared/grpc/grpc-course.service';
import { CloudinaryService } from '@shared/cloudinary/cloudinary.service';
import { saveAchievementLocally } from '@common/utils/file.utils';

@CommandHandler(SaveAchievementFromWebCommand)
export class SaveAchievementFromWebHandler implements ICommandHandler<SaveAchievementFromWebCommand> {
  private readonly logger = new Logger(SaveAchievementFromWebHandler.name);

  constructor(
    @InjectRepository(StudentAchievement)
    private readonly studentAchievementRepository: Repository<StudentAchievement>,
    private readonly grpcCourseService: GrpcCourseService,
    private readonly cloudinaryService: CloudinaryService,
  ) {}

  async execute(command: SaveAchievementFromWebCommand) {
    const { courseId, studentId, studentName, achievementFile } = command;

    this.logger.log(
      `Saving certificate uploaded from web for course: ${courseId}, student: ${studentId}`,
    );

    const count = await this.studentAchievementRepository.count({
      where: { courseId, studentId },
    });
    if (count > 0) {
      throw new BadRequestException('Achievement has already been exported in this course');
    }

    const checkStatus = await this.grpcCourseService.checkStudentCompletedCourse(
      courseId,
      studentId,
    );
    if (!checkStatus.isSuccess) {
      throw new Error(checkStatus.message || 'gRPC CheckStudentCompletedCourse call failed');
    }

    if (!checkStatus.isCompleted) {
      throw new BadRequestException('Student has not completed the course');
    }

    const localResult = saveAchievementLocally(achievementFile, studentName);

    // Upload to Cloudinary & save to DB in background (same pattern as original Express service)
    setImmediate(() => {
      void (async () => {
        try {
          await this.uploadAndSaveAchievement(courseId, studentId, localResult.localPath);
        } catch (error: unknown) {
          const err = error as Error;
          this.logger.error(`Background upload task failed for student ${studentId}`, err.stack);
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
   * Internal: upload the locally saved file to Cloudinary and persist to DB.
   */
  private async uploadAndSaveAchievement(
    courseId: string,
    studentId: number,
    localPath: string,
  ): Promise<void> {
    this.logger.log(`Background uploading to Cloudinary from path: ${localPath}`);

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
    this.logger.log(`Achievement saved to database for course: ${courseId}, student: ${studentId}`);
  }
}
