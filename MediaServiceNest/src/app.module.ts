import { Module } from '@nestjs/common';
import { ConfigModule, ConfigService } from '@nestjs/config';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AppController } from './app.controller';
import { AppService } from './app.service';

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
        ssl: configService.get<string>('DB_SSL') === 'true'
          ? {
              rejectUnauthorized: configService.get<string>('DB_TRUST_CERT') !== 'true',
            }
          : false,
      }),
    }),
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
