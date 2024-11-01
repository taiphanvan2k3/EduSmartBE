using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    /// <summary>
    /// Check whether a student has completed a lesson
    /// </summary>
    public class LessonTracking : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; }

        public long StudentId { get; set; }

        [Comment("The time spent on the lesson in minutes")]
        public long TimeSpent { get; set; }
    }
}