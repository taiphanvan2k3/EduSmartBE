export class GetAchievementExportStatusQuery {
  constructor(
    public readonly courseId: string,
    public readonly studentId: number,
  ) {}
}
