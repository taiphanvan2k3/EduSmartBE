using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Services.LessonManagement.QuizLesson.Schemas
{
    public class QuizAnswerCreateDto
    {
        [Required]
        public string Answer { get; set; }

        public bool IsCorrect { get; set; }

        public string Explanation { get; set; }
    }
}