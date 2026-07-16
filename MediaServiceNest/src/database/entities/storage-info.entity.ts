import { Entity, Column, PrimaryColumn } from 'typeorm';

@Entity({ name: 'StorageInfos' })
export class StorageInfo {
  @PrimaryColumn({ name: 'UserId' })
  userId: number;

  @Column({ name: 'UsedStorage', type: 'numeric' })
  usedStorage: number;

  @Column({ name: 'MaximumStorage', type: 'numeric' })
  maximumStorage: number;
}
