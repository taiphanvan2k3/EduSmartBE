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
import { CommandBus, QueryBus } from '@nestjs/cqrs';
import { ApiTags, ApiBearerAuth, ApiOperation, ApiResponse, ApiQuery } from '@nestjs/swagger';
import { JwtAuthGuard } from '../../common/guards/jwt-auth.guard';
import { RolesGuard } from '../../common/guards/roles.guard';
import { Roles } from '../../common/decorators/roles.decorator';
import { CreateCourseTemplateDto } from './dto/create-course-template.dto';
import { UpdateCourseTemplateDto } from './dto/update-course-template.dto';
import { GetDefaultCourseTemplateQuery } from './queries/get-default-course-template.query';
import { GetCourseTemplateByIdQuery } from './queries/get-course-template-by-id.query';
import { CreateCourseTemplateCommand } from './commands/create-course-template.command';
import { UpdateCourseTemplateCommand } from './commands/update-course-template.command';
import { DeleteCourseTemplateCommand } from './commands/delete-course-template.command';

@ApiTags('Course Templates')
@ApiBearerAuth('BearerAuth')
@UseGuards(JwtAuthGuard, RolesGuard)
@Roles('Teacher')
@Controller('api/course-templates')
export class CourseTemplateController {
  constructor(
    private readonly commandBus: CommandBus,
    private readonly queryBus: QueryBus,
  ) {}

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
  async getDefaultCourseTemplate(@Query('courseId') courseId: string): Promise<unknown> {
    if (!courseId) {
      throw new BadRequestException('Invalid request query');
    }
    return this.queryBus
      .execute(new GetDefaultCourseTemplateQuery(courseId))
      .then((template: unknown) => ({ courseTemplate: template }));
  }

  @Get(':id')
  @ApiOperation({ summary: 'Get the course template information by ID' })
  @ApiResponse({ status: 200, description: 'Course template details' })
  @ApiResponse({ status: 404, description: 'Course template not found' })
  async getCourseTemplateById(@Param('id') id: string): Promise<unknown> {
    return this.queryBus
      .execute(new GetCourseTemplateByIdQuery(id))
      .then((template: unknown) => ({ courseTemplate: template }));
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
  async createCourseTemplate(@Body() dto: CreateCourseTemplateDto): Promise<unknown> {
    return this.commandBus
      .execute(new CreateCourseTemplateCommand(dto))
      .then((id: unknown) => ({ id }));
  }

  @Put(':id')
  @ApiOperation({ summary: 'Update the course template for the course' })
  @ApiResponse({ status: 200, description: 'Updated course template details' })
  @ApiResponse({ status: 404, description: 'Course template not found' })
  async updateCourseTemplate(
    @Param('id') id: string,
    @Body() dto: UpdateCourseTemplateDto,
  ): Promise<unknown> {
    return this.commandBus
      .execute(new UpdateCourseTemplateCommand(id, dto))
      .then((template: unknown) => ({ courseTemplate: template }));
  }

  @Delete(':id')
  @ApiOperation({ summary: 'Delete the course template for the course' })
  @ApiResponse({ status: 200, description: 'ID of deleted course template' })
  @ApiResponse({ status: 404, description: 'Course template not found' })
  async deleteCourseTemplate(@Param('id') id: string): Promise<unknown> {
    return this.commandBus
      .execute(new DeleteCourseTemplateCommand(id))
      .then((deletedId: unknown) => ({ id: deletedId }));
  }
}
