export class GenerateAchievementCommand {
  constructor(
    public readonly courseId: string,
    public readonly studentId: number,
  ) {}
}
