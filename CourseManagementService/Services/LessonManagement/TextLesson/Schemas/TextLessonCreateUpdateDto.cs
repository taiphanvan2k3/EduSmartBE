using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Services.LessonManagement.TextLesson.Schemas
{
    public class TextLessonCreateUpdateDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid ChapterId { get; set; }

        public bool IsPublished { get; set; } = false;

        public bool IsCommentAllowed { get; set; } = true;

        public bool IsRatingAllowed { get; set; } = true;
    }
}