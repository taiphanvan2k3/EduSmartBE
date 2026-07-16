import { Express } from 'express';

export class UploadFileCommand {
  constructor(
    public readonly userId: number,
    public readonly file: Express.Multer.File,
  ) {}
}
