import { CreateCourseTemplateDto } from '../dto/create-course-template.dto';

export class CreateCourseTemplateCommand {
  constructor(public readonly dto: CreateCourseTemplateDto) {}
}
