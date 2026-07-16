import { Entity, Column, PrimaryColumn, CreateDateColumn } from 'typeorm';

@Entity({ name: 'StudentAchievements' })
export class StudentAchievement {
  @PrimaryColumn({ name: 'Id', type: 'uuid' })
  id: string;

  @Column({ name: 'CourseId', type: 'uuid' })
  courseId: string;

  @Column({ name: 'StudentId' })
  studentId: number;

  @Column({ name: 'AchievementURL' })
  achievementUrl: string;

  @CreateDateColumn({ name: 'CreatedAt' })
  createdAt: Date;
}
