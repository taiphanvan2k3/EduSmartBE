import { IsString, IsNotEmpty } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';

export class TextStyleDto {
  @ApiProperty({ example: 'Arial' })
  @IsString()
  @IsNotEmpty()
  fontFamily: string;

  @ApiProperty({ example: '12px' })
  @IsString()
  @IsNotEmpty()
  fontSize: string;

  @ApiProperty({ example: '#000000' })
  @IsString()
  @IsNotEmpty()
  color: string;
}
