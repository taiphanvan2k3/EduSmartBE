import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CqrsModule } from '@nestjs/cqrs';
import { AchievementController } from './achievement.controller';
import { CanvasService } from './services/canvas.service';
import { StudentAchievement } from '@shared/database/entities/student-achievement.entity';
import { CourseAchievementTemplate } from '@shared/database/entities/course-achievement-template.entity';
import { GrpcModule } from '@shared/grpc/grpc.module';
import { CloudinaryModule } from '@shared/cloudinary/cloudinary.module';
import { CourseTemplateModule } from '@modules/course-template/course-template.module';
// Handlers
import { GetAchievementExportStatusHandler } from './handlers/get-achievement-export-status.handler';
import { GenerateAchievementHandler } from './handlers/generate-achievement.handler';
import { SaveAchievementFromWebHandler } from './handlers/save-achievement-from-web.handler';

@Module({
  imports: [
    CqrsModule,
    TypeOrmModule.forFeature([StudentAchievement, CourseAchievementTemplate]),
    GrpcModule,
    CloudinaryModule,
    CourseTemplateModule,
  ],
  controllers: [AchievementController],
  providers: [
    CanvasService,
    // CQRS Handlers
    GetAchievementExportStatusHandler,
    GenerateAchievementHandler,
    SaveAchievementFromWebHandler,
  ],
})
export class AchievementModule {}
