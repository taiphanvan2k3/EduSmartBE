import { Module } from '@nestjs/common';
import { GrpcCourseService } from './grpc-course.service';

@Module({
  providers: [GrpcCourseService],
  exports: [GrpcCourseService],
})
export class GrpcModule {}
