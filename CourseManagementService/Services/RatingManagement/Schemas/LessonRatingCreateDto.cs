using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class LessonRatingCreateDto : IValidatableObject
    {
        public bool IsLike { get; set; }

        public Guid LessonId { get; set; }

        public string Comment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsLike && string.IsNullOrWhiteSpace(Comment))
            {
                yield return new ValidationResult("Comment is required when rating is dislike", [nameof(Comment)]);
            }
        }
    }
}