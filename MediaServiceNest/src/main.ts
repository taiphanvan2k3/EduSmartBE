import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { HttpExceptionFilter } from './common/filters/http-exception.filter';
import { ValidationPipe } from '@nestjs/common';
import { SwaggerModule, DocumentBuilder } from '@nestjs/swagger';
import * as express from 'express';
import compression from 'compression';
import * as path from 'path';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  // Enable CORS
  app.enableCors();

  // Set global prefix to match Express base path
  app.setGlobalPrefix('media-service');

  // Parse JSON and urlencoded payloads (standard Express settings)
  app.use(express.json());
  app.use(express.urlencoded({ extended: true }));

  // Compression middleware
  app.use(
    compression({
      level: 6,
      threshold: 100 * 1000, // > 100kb
      filter: (req: any, res: any) => {
        if (req.headers && req.headers['x-no-compression']) {
          return false;
        }
        return compression.filter(req, res);
      },
    }),
  );

  // Static files serving (under /media-service prefix)
  app.use(
    '/media-service',
    express.static(path.join(__dirname, '..', 'public', 'media-service')),
  );

  // Global validation pipe using class-validator
  app.useGlobalPipes(
    new ValidationPipe({
      whitelist: true,
      transform: true,
    }),
  );

  // Global Exception Filter
  app.useGlobalFilters(new HttpExceptionFilter());

  // Setup Swagger API documentation
  const config = new DocumentBuilder()
    .setTitle('EduSmart MediaService API')
    .setDescription('Media management and achievement operations')
    .setVersion('1.0')
    .addBearerAuth(
      {
        type: 'http',
        scheme: 'bearer',
        bearerFormat: 'JWT',
        name: 'JWT',
        description: 'Enter JWT token',
        in: 'header',
      },
      'BearerAuth', // Name matches the Swagger security requirement name
    )
    .build();

  const document = SwaggerModule.createDocument(app, config);
  SwaggerModule.setup('media-service/swagger', app, document);

  const port = process.env.PORT || 7154;
  await app.listen(port, '0.0.0.0');
  console.log(
    `Server started on port http://localhost:${port}/media-service/swagger`,
  );
}
bootstrap();
