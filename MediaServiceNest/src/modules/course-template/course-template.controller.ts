import {
  Controller,
  Get,
  Post,
  Put,
  Delete,
  Body,
  Param,
  Query,
  UseGuards,
  HttpCode,
  HttpStatus,
  BadRequestException,
} from '@nestjs/common';
import {
  ApiTags,
  ApiBearerAuth,
  ApiOperation,
  ApiResponse,
  ApiQuery,
} from '@nestjs/swagger';
import { CourseTemplateService } from './course-template.service';
import { CreateCourseTemplateDto } from './dto/create-course-template.dto';
import { UpdateCourseTemplateDto } from './dto/update-course-template.dto';
import { JwtAuthGuard } from '../../common/guards/jwt-auth.guard';
import { RolesGuard } from '../../common/guards/roles.guard';
import { Roles } from '../../common/decorators/roles.decorator';

@ApiTags('Course Templates')
@ApiBearerAuth('BearerAuth')
@UseGuards(JwtAuthGuard, RolesGuard)
@Roles('Teacher')
@Controller('api/course-templates')
export class CourseTemplateController {
  constructor(private readonly courseTemplateService: CourseTemplateService) {}

  @Get('default-template')
  @ApiOperation({ summary: 'Get the default course template for the course' })
  @ApiQuery({
    name: 'courseId',
    required: true,
    description: 'ID of the course',
  })
  @ApiResponse({
    status: 200,
    description: 'The default course template details',
  })
  async getDefaultCourseTemplate(@Query('courseId') courseId: string) {
    if (!courseId) {
      throw new BadRequestException('Invalid request query');
    }
    const template =
      await this.courseTemplateService.getDefaultCourseTemplateInCourse(
        courseId,
      );
    return { courseTemplate: template };
  }

  @Get(':id')
  @ApiOperation({ summary: 'Get the course template information by ID' })
  @ApiResponse({ status: 200, description: 'Course template details' })
  @ApiResponse({ status: 404, description: 'Course template not found' })
  async getCourseTemplateById(@Param('id') id: string) {
    const template = await this.courseTemplateService.getCourseTemplateById(id);
    return { courseTemplate: template };
  }

  @Post()
  @HttpCode(HttpStatus.OK)
  @ApiOperation({
    summary: 'Teacher creates a new course template for the course',
  })
  @ApiResponse({ status: 200, description: 'ID of created course template' })
  @ApiResponse({
    status: 400,
    description: 'Invalid body or template already exists',
  })
  async createCourseTemplate(@Body() dto: CreateCourseTemplateDto) {
    const id = await this.courseTemplateService.createTemplateForCourse(dto);
    return { id };
  }

  @Put(':id')
  @ApiOperation({ summary: 'Update the course template for the course' })
  @ApiResponse({ status: 200, description: 'Updated course template details' })
  @ApiResponse({ status: 404, description: 'Course template not found' })
  async updateCourseTemplate(
    @Param('id') id: string,
    @Body() dto: UpdateCourseTemplateDto,
  ) {
    const template = await this.courseTemplateService.updateTemplateForCourse(
      id,
      dto,
    );
    return { courseTemplate: template };
  }

  @Delete(':id')
  @ApiOperation({ summary: 'Delete the course template for the course' })
  @ApiResponse({ status: 200, description: 'ID of deleted course template' })
  @ApiResponse({ status: 404, description: 'Course template not found' })
  async deleteCourseTemplate(@Param('id') id: string) {
    const deletedId = await this.courseTemplateService.deleteCourseTemplate(id);
    return { id: deletedId };
  }
}
