namespace CourseManagementService.Services.CourseManagement.Student.Schemas
{
    public class CompletedCourseInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AchievementURL { get; set; }

        public DateTimeOffset? CreateAt { get; set; }
    }
}