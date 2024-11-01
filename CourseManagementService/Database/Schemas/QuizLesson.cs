using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Database.Schemas
{
    public class QuizLesson : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Question { get; set; }

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; }

        public bool IsMultipleChoice { get; set; }

        public virtual ICollection<QuizAnswer> Answers { get; set; }
    }
}