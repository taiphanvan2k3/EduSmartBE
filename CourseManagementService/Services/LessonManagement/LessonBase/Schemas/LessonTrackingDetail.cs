namespace CourseManagementService.Services.LessonManagement.LessonBase.Schemas
{
    public class LessonTrackingDetail
    {
        public Guid LessonId { get; set; }

        /// <summary>
        /// The time spent for the lesson (in seconds)
        /// </summary>
        public long TimeSpent { get; set; }

        public DateTimeOffset LastAccessed { get; set; }
    }
}