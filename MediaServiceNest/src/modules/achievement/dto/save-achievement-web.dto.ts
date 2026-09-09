import { IsUUID, IsInt, IsString, IsNotEmpty, Min } from 'class-validator';
import { Type } from 'class-transformer';
import { ApiProperty } from '@nestjs/swagger';

export class SaveAchievementWebDto {
  @ApiProperty({ example: '320e8b20-0e48-4bd6-8da0-33836ffc2a75' })
  @IsUUID()
  courseId: string;

  @ApiProperty({ example: 1 })
  @IsInt()
  @Min(1)
  @Type(() => Number)
  studentId: number;

  @ApiProperty({ example: 'Nguyen Van A' })
  @IsString()
  @IsNotEmpty()
  studentName: string;
}
