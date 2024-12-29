namespace CourseManagementService.Services.NotificationManagement.Schemas
{
    public class NotificationMetaData
    {
        public string CourseName { get; set; }

        public string LessonName { get; set; }

        public Guid? DiscussionId { get; set; }

        public Guid? LessonId { get; set; }
    }
}