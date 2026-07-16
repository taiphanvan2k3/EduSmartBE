import { UpdateCourseTemplateDto } from '../dto/update-course-template.dto';

export class UpdateCourseTemplateCommand {
  constructor(
    public readonly id: string,
    public readonly dto: UpdateCourseTemplateDto,
  ) {}
}
