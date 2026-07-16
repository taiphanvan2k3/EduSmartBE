import { Controller, Get, UseGuards } from '@nestjs/common';
import { ApiTags, ApiBearerAuth, ApiOperation, ApiResponse } from '@nestjs/swagger';
import { AchievementTemplateService } from './achievement-template.service';
import { JwtAuthGuard } from '../../common/guards/jwt-auth.guard';

@ApiTags('Templates')
@ApiBearerAuth('BearerAuth')
@UseGuards(JwtAuthGuard)
@Controller('api/templates')
export class AchievementTemplateController {
  constructor(private readonly templateService: AchievementTemplateService) {}

  @Get()
  @ApiOperation({ summary: 'Get all templates that can be used to create achievements' })
  @ApiResponse({ status: 200, description: 'List of achievement templates' })
  async getAllTemplates() {
    return this.templateService.getAllTemplates();
  }
}
