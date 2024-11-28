namespace CourseManagementService.Database.Schemas.Discussion
{
    public class Discussion : BaseEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsAnswered { get; set; }

        public DateTimeOffset DeletedAt { get; set; }

        public int UserId { get; set; }

        public DiscussionType Type { get; set; }

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; }
    }
}