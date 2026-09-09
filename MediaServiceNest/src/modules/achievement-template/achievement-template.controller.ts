import { Controller, Get, UseGuards } from '@nestjs/common';
import { QueryBus } from '@nestjs/cqrs';
import { ApiTags, ApiBearerAuth, ApiOperation, ApiResponse } from '@nestjs/swagger';
import { JwtAuthGuard } from '../../common/guards/jwt-auth.guard';
import { RolesGuard } from '../../common/guards/roles.guard';
import { Roles } from '../../common/decorators/roles.decorator';
import { GetAllTemplatesQuery } from './queries/get-all-templates.query';

@ApiTags('Achievement Templates')
@ApiBearerAuth('BearerAuth')
@UseGuards(JwtAuthGuard, RolesGuard)
@Roles('Teacher')
@Controller('api/templates')
export class AchievementTemplateController {
  constructor(private readonly queryBus: QueryBus) {}

  @Get()
  @ApiOperation({ summary: 'Get all available achievement templates' })
  @ApiResponse({ status: 200, description: 'List of achievement templates' })
  async getAllTemplates(): Promise<unknown> {
    return this.queryBus.execute(new GetAllTemplatesQuery());
  }
}
