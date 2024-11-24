using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    public class QuizAnswer : BaseEntity
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Answer { get; set; }

        public bool IsCorrect { get; set; }

        [Comment("Explanation why the answer is correct or incorrect")]
        [MaxLength(500)]
        public string Explanation { get; set; }

        public Guid QuizLessonId { get; set; }

        public QuizLesson QuizLesson { get; set; }
    }
}