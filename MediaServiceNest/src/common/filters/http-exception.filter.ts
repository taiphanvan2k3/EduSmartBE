import {
  ExceptionFilter,
  Catch,
  ArgumentsHost,
  HttpException,
  HttpStatus,
  Logger,
} from '@nestjs/common';
import { Request, Response } from 'express';
import * as fs from 'fs';
import * as path from 'path';
import { format } from 'date-fns';
import { v4 as uuidv4 } from 'uuid';

@Catch()
export class HttpExceptionFilter implements ExceptionFilter {
  private readonly logger = new Logger(HttpExceptionFilter.name);
  private readonly logDirectory = path.join(__dirname, '..', '..', 'Logs');
  private readonly logFileName = path.join(this.logDirectory, 'logs.log');

  logEvents(msg: string) {
    const dateTime = format(new Date(), 'dd-MM-yyyy\tHH:mm:ss');
    const contentLog = `${dateTime} ----- ${msg}\n`;

    try {
      if (!fs.existsSync(this.logDirectory)) {
        fs.mkdirSync(this.logDirectory, { recursive: true });
      }
      fs.appendFileSync(this.logFileName, contentLog);
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error('Error writing log file', err.message);
    }
  }

  catch(exception: unknown, host: ArgumentsHost) {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse<Response>();
    const request = ctx.getRequest<Request>();

    const status =
      exception instanceof HttpException
        ? exception.getStatus()
        : HttpStatus.INTERNAL_SERVER_ERROR;

    let errorName = 'InternalServerError';
    if (exception instanceof HttpException) {
      errorName = exception.name;
    } else if (exception instanceof Error) {
      errorName = exception.name;
    }

    // Extract message
    let message = 'An unexpected error occurred';
    if (exception instanceof HttpException) {
      const resContent: unknown = exception.getResponse();
      if (typeof resContent === 'object' && resContent !== null) {
        const resObj = resContent as Record<string, any>;
        message = (resObj.message as string) || exception.message;
        if (Array.isArray(message)) {
          message = message.join(', '); // If validation error list, join them
        }
      } else if (typeof resContent === 'string') {
        message = resContent;
      } else {
        message = exception.message;
      }
    } else if (exception instanceof Error) {
      message = exception.message;
    }

    // Log the event exactly like original Express error handler
    const errorId = uuidv4();
    this.logEvents(
      `idError ----- ${errorId} ----- ${request.url} ----- ${request.method} ----- ${message}`,
    );

    this.logger.error(
      `[${request.method}] ${request.url} - Status: ${status} - Error: ${message}`,
      exception instanceof Error ? exception.stack : undefined,
    );

    response.status(status).json({
      statusCode: status,
      error: errorName,
      message: message,
    });
  }
}
