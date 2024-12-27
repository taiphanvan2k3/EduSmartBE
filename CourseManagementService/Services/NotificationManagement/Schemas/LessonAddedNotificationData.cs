namespace CourseManagementService.Services.NotificationManagement.Schemas
{
    public class LessonAddedNotificationData
    {
        public Guid CourseId { get; set; }

        public Guid LessonId { get; set; }

        public string LessonName { get; set; }
    }
}