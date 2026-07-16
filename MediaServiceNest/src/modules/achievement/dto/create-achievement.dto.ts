import { TextStyle } from '../services/canvas.service';

export interface CreateAchievementDto {
  templateId: number;
  studentName: string;
  courseName: string;
  teacherName: string;
  studentNameTextStyle: TextStyle;
  courseNameTextStyle: TextStyle;
  dateTextStyle: TextStyle;
  teacherNameTextStyle: TextStyle;
}
