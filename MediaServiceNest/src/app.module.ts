import { Module } from '@nestjs/common';
import { ConfigModule, ConfigService } from '@nestjs/config';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { CloudinaryModule } from './shared/cloudinary/cloudinary.module';
import { GrpcModule } from './shared/grpc/grpc.module';
import { JwtStrategy } from './common/strategies/jwt.strategy';
import { MediaModule } from './modules/media/media.module';
import { AchievementTemplateModule } from './modules/achievement-template/achievement-template.module';
import { CourseTemplateModule } from './modules/course-template/course-template.module';
import { AchievementModule } from './modules/achievement/achievement.module';

@Module({
  imports: [
    // Configure ConfigModule to load .env variables globally
    ConfigModule.forRoot({
      isGlobal: true,
      envFilePath: '.env',
    }),

    // Configure TypeORM asynchronously with PostgreSQL
    TypeOrmModule.forRootAsync({
      imports: [ConfigModule],
      inject: [ConfigService],
      useFactory: (configService: ConfigService) => ({
        type: 'postgres',
        host: configService.get<string>('DB_HOST', 'localhost'),
        port: configService.get<number>('DB_PORT', 5432),
        username: configService.get<string>('DB_USERNAME', 'postgres'),
        password: configService.get<string>('DB_PASSWORD', ''),
        database: configService.get<string>('DB_DATABASE', 'postgres'),
        autoLoadEntities: true,
        synchronize: false, // Do not synchronize schema to avoid altering current DB
        logging: configService.get<string>('NODE_ENV') === 'development',
        ssl:
          configService.get<string>('DB_SSL') === 'true'
            ? {
                rejectUnauthorized: configService.get<string>('DB_TRUST_CERT') !== 'true',
              }
            : false,
      }),
    }),

    CloudinaryModule,
    GrpcModule,
    MediaModule,
    AchievementTemplateModule,
    CourseTemplateModule,
    AchievementModule,
  ],
  controllers: [AppController],
  providers: [AppService, JwtStrategy],
})
export class AppModule {}
