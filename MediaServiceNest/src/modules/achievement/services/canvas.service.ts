import { Injectable, Logger } from '@nestjs/common';
import { createCanvas, loadImage, CanvasRenderingContext2D } from 'canvas';
import * as path from 'path';
import * as fs from 'fs';
import { formatDateTime, createFolderIfNotExist } from '../../../common/utils/file.utils';
import { CreateAchievementDto } from '../dto/create-achievement.dto';

export interface TextStyle {
  fontFamily: string;
  fontSize: string;
  color: string;
}

@Injectable()
export class CanvasService {
  private readonly logger = new Logger(CanvasService.name);

  async createAchievement(
    dto: CreateAchievementDto,
  ): Promise<{ localPath: string; localPathInPublic: string }> {
    const {
      templateId,
      studentName,
      courseName,
      teacherName,
      studentNameTextStyle,
      courseNameTextStyle,
      dateTextStyle,
      teacherNameTextStyle,
    } = dto;

    this.logger.log(`Creating achievement image with templateId: ${templateId}...`);
    try {
      // Resolve path to templates (located in dist/assets/achievement-templates/ after build)
      const templatePath = path.join(
        __dirname,
        '..',
        '..',
        '..',
        'assets',
        'achievement-templates',
        `${templateId}.png`,
      );

      this.logger.log(`Loading template from path: ${templatePath}`);
      // Load the original image
      const image = await loadImage(templatePath);

      // Create canvas and context
      const canvas = createCanvas(image.width, image.height);
      const ctx = canvas.getContext('2d');

      // Draw original template image onto canvas
      ctx.drawImage(image, 0, 0);

      // ====== Student Name ======
      this.setTextStyle(ctx, studentNameTextStyle);
      this.modifyFontSize(
        ctx,
        studentName,
        image.width,
        Number.parseFloat(studentNameTextStyle.fontSize),
      );

      const nameTextWidth = ctx.measureText(studentName).width;
      const nameTextHeight = ctx.measureText(studentName).actualBoundingBoxAscent;
      const namePosition = {
        x: (image.width - nameTextWidth) / 2, // Centered
        y: (templateId === 1 ? 375 : 400) - nameTextHeight / 2,
      };
      ctx.fillText(studentName, namePosition.x, namePosition.y);

      // ====== Course Name ======
      this.setTextStyle(ctx, courseNameTextStyle);
      this.modifyFontSize(
        ctx,
        courseName,
        image.width,
        Number.parseFloat(courseNameTextStyle.fontSize),
      );
      const courseTextWidth = ctx.measureText(courseName).width;
      const coursePosition = {
        x: (image.width - courseTextWidth) / 2, // Centered
        y: 500,
      };
      ctx.fillText(courseName, coursePosition.x, coursePosition.y);

      // ====== Date ======
      ctx.font = `bold 30px "Dancing Script"`;
      ctx.fillStyle = dateTextStyle.color;
      ctx.fillText('Date', 255, 600);

      const date = new Date();
      const dateString = formatDateTime(date);
      this.setTextStyle(ctx, dateTextStyle);
      ctx.fillText(dateString, 227, 648);

      // ====== Teacher Name ======
      ctx.font = `bold 30px "Dancing Script"`;
      ctx.fillStyle = teacherNameTextStyle.color;
      ctx.fillText('Teacher', 825, 600);

      // Centered at x = 857
      this.setTextStyle(ctx, teacherNameTextStyle);
      const teacherNameTextWidth = ctx.measureText(teacherName).width;
      const teacherNamePosition = {
        x: 860 - teacherNameTextWidth / 2,
        y: 648,
      };
      ctx.fillText(teacherName, teacherNamePosition.x, teacherNamePosition.y);

      const buffer = canvas.toBuffer('image/jpeg');

      // Save to public directory
      const localFolderPath = path.join(
        process.cwd(),
        'public',
        'media-service',
        'temp_achievements',
      );
      const fileName = `${studentName}-${date.getTime()}.jpg`;

      createFolderIfNotExist(localFolderPath);

      const achievementLocalPath = path.join(localFolderPath, fileName);
      fs.writeFileSync(achievementLocalPath, buffer);

      this.logger.log('Achievement image saved to local folder successfully');

      return {
        localPath: achievementLocalPath,
        localPathInPublic: `/media-service/temp_achievements/${fileName}`,
      };
    } catch (error: unknown) {
      const err = error as Error;
      this.logger.error('Error generating certificate canvas', err.stack);
      throw error;
    }
  }

  private setTextStyle(ctx: CanvasRenderingContext2D, textStyle: TextStyle) {
    ctx.font = `${textStyle.fontSize} "Dancing Script"`;
    ctx.fillStyle = textStyle.color;
  }

  private modifyFontSize(
    ctx: CanvasRenderingContext2D,
    text: string,
    maxWidth: number,
    currentFontSize: number,
  ) {
    let currentTextWidth = ctx.measureText(text).width;
    while (currentTextWidth >= maxWidth || maxWidth - currentTextWidth <= 200) {
      currentFontSize -= 2;
      ctx.font = `${currentFontSize}px "Dancing Script"`;
      currentTextWidth = ctx.measureText(text).width;
    }
  }
}
