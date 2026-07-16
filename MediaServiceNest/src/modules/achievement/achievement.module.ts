import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AchievementController } from './achievement.controller';
import { AchievementService } from './achievement.service';
import { CanvasService } from './services/canvas.service';
import { StudentAchievement } from '../../shared/database/entities/student-achievement.entity';
import { CourseAchievementTemplate } from '../../shared/database/entities/course-achievement-template.entity';
import { GrpcModule } from '../../shared/grpc/grpc.module';
import { CloudinaryModule } from '../../shared/cloudinary/cloudinary.module';
import { CourseTemplateModule } from '../course-template/course-template.module';

@Module({
  imports: [
    TypeOrmModule.forFeature([StudentAchievement, CourseAchievementTemplate]),
    GrpcModule,
    CloudinaryModule,
    CourseTemplateModule,
  ],
  controllers: [AchievementController],
  providers: [AchievementService, CanvasService],
})
export class AchievementModule {}
