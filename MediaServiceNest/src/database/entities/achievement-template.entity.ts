import { Entity, Column, PrimaryGeneratedColumn } from 'typeorm';

@Entity({ name: 'AchievementTemplates' })
export class AchievementTemplate {
  @PrimaryGeneratedColumn({ name: 'Id' })
  id: number;

  @Column({ name: 'Name', nullable: true })
  name: string;

  @Column({ name: 'ThumbnailURL', nullable: true })
  thumbnailUrl: string;

  @Column({ name: 'TemplateURL' })
  templateUrl: string;
}
