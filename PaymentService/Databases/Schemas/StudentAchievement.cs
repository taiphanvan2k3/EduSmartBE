namespace PaymentService.Databases.Schemas
{
    public class StudentAchievement
    {
        public Guid Id { get; set; }

        public int StudentId { get; set; }

        public Guid CourseId { get; set; }

        public string AchievementURL { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}