import { Injectable, Logger, OnModuleInit } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import * as grpc from '@grpc/grpc-js';
import * as protoLoader from '@grpc/proto-loader';
import * as path from 'path';

export interface CheckStudentCompletedCourseResponse {
  isCompleted: boolean;
  studentName: string;
  courseName: string;
  teacherName: string;
  message: string;
  isSuccess: boolean;
}

export interface CourseGrpcClient {
  CheckStudentCompletedCourse(
    request: { studentId: number; courseId: string },
    callback: (
      err: grpc.ServiceError | null,
      response: CheckStudentCompletedCourseResponse,
    ) => void,
  ): void;
}

@Injectable()
export class GrpcCourseService implements OnModuleInit {
  private readonly logger = new Logger(GrpcCourseService.name);
  private client: CourseGrpcClient | null = null;

  constructor(private readonly configService: ConfigService) {}

  onModuleInit() {
    const PROTO_PATH = path.join(__dirname, 'protos', 'course.proto');

    this.logger.log(`Loading proto file from: ${PROTO_PATH}`);

    // Tải file .proto
    const packageDefinition = protoLoader.loadSync(PROTO_PATH, {
      keepCase: true,
      longs: String,
      enums: String,
      defaults: true,
      oneofs: true,
    });

    const isProd = this.configService.get<string>('NODE_ENV') === 'production';
    const grpcEndpoint = isProd ? 'courseservice:10002' : 'localhost:10002';

    this.logger.log(`Connecting to gRPC Course service at ${grpcEndpoint}...`);

    const proto = grpc.loadPackageDefinition(packageDefinition) as unknown as {
      Course: new (address: string, credentials: grpc.ChannelCredentials) => CourseGrpcClient;
    };

    this.client = new proto.Course(grpcEndpoint, grpc.credentials.createInsecure());
  }

  checkStudentCompletedCourse(
    courseId: string,
    studentId: number,
  ): Promise<CheckStudentCompletedCourseResponse> {
    this.logger.log(
      `Calling CheckStudentCompletedCourse for courseId: ${courseId}, studentId: ${studentId}`,
    );
    return new Promise((resolve, reject) => {
      if (!this.client) {
        reject(new Error('gRPC client is not initialized'));
        return;
      }
      this.client.CheckStudentCompletedCourse(
        { studentId, courseId },
        (err: grpc.ServiceError | null, response: CheckStudentCompletedCourseResponse) => {
          if (err) {
            this.logger.error('gRPC CheckStudentCompletedCourse call failed', err.message);
            reject(err);
          } else {
            resolve(response);
          }
        },
      );
    });
  }
}
