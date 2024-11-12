using System.ComponentModel.DataAnnotations;
using CourseManagementService.Services.LessonManagement.VideoLesson.Schemas;

namespace CourseManagementService.Services.LessonManagement.QuizLesson.Schemas
{
    public class QuizLessonUpdateDto : TextLessonCreateUpdateDto, IValidatableObject
    {
        [Obsolete("Description is hidden in QuizLessonCreateDto", true)]
        public new string Description { get; }

        [Required]
        public string Question { get; set; }

        public bool IsMultipleChoice { get; set; }

        public List<QuizAnswerUpdateDto> Answers { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Answers == null || Answers.Count < 2)
            {
                yield return new ValidationResult("At least 2 answers are required", [nameof(Answers)]);
            }

            if (IsMultipleChoice && !Answers.Any(a => a.IsCorrect))
            {
                yield return new ValidationResult("At least 1 correct answer is required", [nameof(Answers)]);
            }

            if (!IsMultipleChoice && Answers.Count(a => a.IsCorrect) != 1)
            {
                yield return new ValidationResult("Exactly 1 correct answer is required", [nameof(Answers)]);
            }

            if (DurationInSeconds <= 0)
            {
                yield return new ValidationResult("Estimated learning time must be greater than 0", [nameof(DurationInSeconds)]);
            }
        }
    }
}