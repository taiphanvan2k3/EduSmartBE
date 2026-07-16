import * as fs from 'fs';
import * as path from 'path';

export const formatDateTime = (date: Date): string => {
  const day = date.getDate();
  const month = date.getMonth() + 1;
  const year = date.getFullYear();

  let formattedDate = '';
  if (day < 10) {
    formattedDate += '0';
  }
  formattedDate += `${day}/`;

  if (month < 10) {
    formattedDate += '0';
  }
  formattedDate += `${month}/${year}`;

  return formattedDate;
};

export const createFolderIfNotExist = (folderPath: string): void => {
  if (!fs.existsSync(folderPath)) {
    fs.mkdirSync(folderPath, { recursive: true });
  }
};

export const saveAchievementLocally = (
  achievementFile: Express.Multer.File,
  studentName: string,
): { localPath: string; localPathInPublic: string } => {
  const date = new Date();
  const pathToProject = process.cwd();
  const localFolderPath = path.join(
    pathToProject,
    'public',
    'media-service',
    'temp_achievements',
  );

  createFolderIfNotExist(localFolderPath);

  const fileExtension = achievementFile.originalname.split('.').pop();
  const fileName = `${studentName}_${date.getTime()}.${fileExtension}`;

  const localPath = path.join(localFolderPath, fileName);
  const localPathInPublic = `/media-service/temp_achievements/${fileName}`;

  fs.writeFileSync(localPath, achievementFile.buffer);

  return {
    localPath,
    localPathInPublic,
  };
};
