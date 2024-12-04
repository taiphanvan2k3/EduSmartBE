namespace CourseManagementService.Database.Schemas
{
    public class Bookmark
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public Guid LessonId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public virtual Lesson Lesson { get; set; }
    }
}