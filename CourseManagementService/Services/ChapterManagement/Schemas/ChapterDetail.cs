using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;

namespace CourseManagementService.Services.ChapterManagement.Schemas
{
    public class ChapterDetail
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public Guid CourseId { get; set; }

        public string Duration { get; set; }

        public List<LessonDetail> Lessons { get; set; }
    }
}