import { Entity, Column, PrimaryColumn, ManyToOne, JoinColumn } from 'typeorm';
import { AchievementTemplate } from './achievement-template.entity';

@Entity({ name: 'CourseAchievementTemplates' })
export class CourseAchievementTemplate {
  @PrimaryColumn({ name: 'Id', type: 'uuid' })
  id: string;

  @Column({ name: 'CourseId', type: 'uuid' })
  courseId: string;

  @Column({ name: 'AchievementTemplateId' })
  achievementTemplateId: number;

  @ManyToOne(() => AchievementTemplate)
  @JoinColumn({ name: 'AchievementTemplateId' })
  achievementTemplate: AchievementTemplate;

  @Column({ name: 'CourseNameTextStyle', type: 'jsonb' })
  courseNameTextStyle: Record<string, any>;

  @Column({ name: 'StudentNameTextStyle', type: 'jsonb' })
  studentNameTextStyle: Record<string, any>;

  @Column({ name: 'DateTextStyle', type: 'jsonb' })
  dateTextStyle: Record<string, any>;

  @Column({ name: 'TeacherNameTextStyle', type: 'jsonb' })
  teacherNameTextStyle: Record<string, any>;

  @Column({ name: 'IsDefault', default: false })
  isDefault: boolean;
}
