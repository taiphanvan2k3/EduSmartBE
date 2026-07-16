import {
  Controller,
  Get,
  Post,
  Body,
  Query,
  UseGuards,
  UseInterceptors,
  UploadedFile,
  BadRequestException,
  HttpCode,
  HttpStatus,
} from '@nestjs/common';
import { CommandBus, QueryBus } from '@nestjs/cqrs';
import { FileInterceptor } from '@nestjs/platform-express';
import {
  ApiTags,
  ApiBearerAuth,
  ApiOperation,
  ApiResponse,
  ApiQuery,
  ApiConsumes,
  ApiBody,
} from '@nestjs/swagger';
import { GenerateAchievementDto } from './dto/generate-achievement.dto';
import { SaveAchievementWebDto } from './dto/save-achievement-web.dto';
import { JwtAuthGuard } from '../../common/guards/jwt-auth.guard';
import { GetAchievementExportStatusQuery } from './queries/get-achievement-export-status.query';
import { GenerateAchievementCommand } from './commands/generate-achievement.command';
import { SaveAchievementFromWebCommand } from './commands/save-achievement-from-web.command';
import {
  AchievementExportStatusDto,
  GeneratedAchievementDto,
} from './dto/achievement-response.dto';

@ApiTags('Achievements')
@ApiBearerAuth('BearerAuth')
@UseGuards(JwtAuthGuard)
@Controller('api/achievements')
export class AchievementController {
  constructor(
    private readonly commandBus: CommandBus,
    private readonly queryBus: QueryBus,
  ) {}

  @Get('export-status')
  @ApiOperation({
    summary: 'Get certificate export status of a student in a course',
  })
  @ApiQuery({
    name: 'courseId',
    required: true,
    description: 'ID of the course',
  })
  @ApiQuery({
    name: 'studentId',
    required: true,
    description: 'ID of the student',
  })
  @ApiResponse({ status: 200, description: 'Return export details' })
  async getExportStatus(
    @Query('courseId') courseId: string,
    @Query('studentId') studentId: string,
  ): Promise<AchievementExportStatusDto> {
    if (!courseId || !studentId) {
      throw new BadRequestException('Invalid request query');
    }
    const studentIdNum = Number.parseInt(studentId, 10);
    if (Number.isNaN(studentIdNum)) {
      throw new BadRequestException('studentId must be a valid number');
    }
    return this.queryBus.execute<GetAchievementExportStatusQuery, AchievementExportStatusDto>(
      new GetAchievementExportStatusQuery(courseId, studentIdNum),
    );
  }

  @Post('generate-achievement')
  @HttpCode(HttpStatus.OK)
  @ApiOperation({ summary: 'Generate certificate image for student' })
  @ApiResponse({ status: 200, description: 'Generated certificate info' })
  async generateAchievement(@Body() dto: GenerateAchievementDto): Promise<GeneratedAchievementDto> {
    return this.commandBus.execute<GenerateAchievementCommand, GeneratedAchievementDto>(
      new GenerateAchievementCommand(dto.courseId, dto.studentId),
    );
  }

  @Post('save-exported-achievement-web')
  @HttpCode(HttpStatus.OK)
  @UseInterceptors(FileInterceptor('file'))
  @ApiConsumes('multipart/form-data')
  @ApiOperation({
    summary: 'Save uploaded certificate (exported by Web frontend)',
  })
  @ApiBody({
    schema: {
      type: 'object',
      properties: {
        file: {
          type: 'string',
          format: 'binary',
          description: 'The certificate file to save',
        },
        courseId: {
          type: 'string',
          description: 'ID of the course',
        },
        studentId: {
          type: 'number',
          description: 'ID of the student',
        },
        studentName: {
          type: 'string',
          description: 'Name of the student',
        },
      },
    },
  })
  @ApiResponse({ status: 200, description: 'Saved certificate details' })
  async saveExportedStudentAchievementFromWeb(
    @Body() dto: SaveAchievementWebDto,
    @UploadedFile() file: Express.Multer.File,
  ): Promise<unknown> {
    if (!file) {
      throw new BadRequestException('No file provided!');
    }
    return this.commandBus.execute(
      new SaveAchievementFromWebCommand(dto.courseId, dto.studentId, dto.studentName, file),
    );
  }
}
