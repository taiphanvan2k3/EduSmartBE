export class SaveAchievementFromWebCommand {
  constructor(
    public readonly courseId: string,
    public readonly studentId: number,
    public readonly studentName: string,
    public readonly achievementFile: Express.Multer.File,
  ) {}
}
