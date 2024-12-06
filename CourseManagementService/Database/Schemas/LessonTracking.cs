using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    /// <summary>
    /// Check the time spent on each lesson
    /// </summary>
    public class LessonTracking : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; }

        /// <summary>
        /// Save the course id to query faster
        /// </summary>
        public Guid CourseId { get; set; }

        public Course Course { get; set; }

        public long StudentId { get; set; }

        [Comment("The time spent on the lesson in seconds")]
        public int TimeSpent { get; set; }

        public bool IsCompleted { get; set; }
    }
}