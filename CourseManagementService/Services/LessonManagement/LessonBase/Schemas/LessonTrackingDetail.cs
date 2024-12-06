namespace CourseManagementService.Services.LessonManagement.LessonBase.Schemas
{
    public class LessonTrackingDetail
    {
        public Guid LessonId { get; set; }

        public int LessonOrder { get; set; }

        public int ChapterOrder { get; set; }

        /// <summary>
        /// The time spent for the lesson (in seconds)
        /// </summary>
        public int TimeSpent { get; set; }

        public bool IsCompleted { get; set; }

        public DateTimeOffset LastAccessed { get; set; }
    }
}