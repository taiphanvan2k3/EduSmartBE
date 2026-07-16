import {
  IsBoolean,
  IsUUID,
  IsInt,
  Min,
  Max,
  ValidateNested,
  IsNotEmpty,
} from 'class-validator';
import { Type } from 'class-transformer';
import { ApiProperty } from '@nestjs/swagger';
import { TextStyleDto } from './text-style.dto';

export class CreateCourseTemplateDto {
  @ApiProperty({ example: true })
  @IsBoolean()
  isDefault: boolean;

  @ApiProperty({ example: '320e8b20-0e48-4bd6-8da0-33836ffc2a75' })
  @IsUUID()
  @IsNotEmpty()
  courseId: string;

  @ApiProperty({ example: 1, minimum: 1, maximum: 6 })
  @IsInt()
  @Min(1)
  @Max(6)
  templateId: number;

  @ApiProperty({ type: TextStyleDto })
  @ValidateNested()
  @Type(() => TextStyleDto)
  @IsNotEmpty()
  studentNameTextStyle: TextStyleDto;

  @ApiProperty({ type: TextStyleDto })
  @ValidateNested()
  @Type(() => TextStyleDto)
  @IsNotEmpty()
  courseNameTextStyle: TextStyleDto;

  @ApiProperty({ type: TextStyleDto })
  @ValidateNested()
  @Type(() => TextStyleDto)
  @IsNotEmpty()
  dateTextStyle: TextStyleDto;

  @ApiProperty({ type: TextStyleDto })
  @ValidateNested()
  @Type(() => TextStyleDto)
  @IsNotEmpty()
  teacherNameTextStyle: TextStyleDto;
}
