import { TextStyle } from '../services/canvas.service';

export interface ExportedAchievementDto {
  achievementURL: string;
  createdAt: Date;
}

export interface AchievementExportInfoDto {
  templateId: number;
  templateURL: string | null;
  studentNameTextStyle: TextStyle;
  courseNameTextStyle: TextStyle;
  dateTextStyle: TextStyle;
  teacherNameTextStyle: TextStyle;
  courseName: string;
  studentName: string;
  teacherName: string;
}

export interface AchievementExportStatusDto {
  canExport: boolean;
  exportedAchievement: ExportedAchievementDto | null;
  achievementExportInfo: AchievementExportInfoDto | null;
}

export interface GeneratedAchievementDto {
  courseId: string;
  localPath: string;
  localPathInPublic: string;
  createdAt: string;
}
