using CourseManagementService.Services.ChapterManagement.Schemas;

namespace CourseManagementService.Services.NoteManagement.Schemas
{
    public class NoteDetail
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string Comment { get; set; }

        public Guid LessonId { get; set; }

        public int TimeMilestone { get; set; }

        public string LessonType { get; set; }

        public string LessonName { get; set; }

        public SimpleChapterInfo ChapterInfo { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}