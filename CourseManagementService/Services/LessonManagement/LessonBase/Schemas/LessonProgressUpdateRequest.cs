using System.ComponentModel.DataAnnotations;
using CourseManagementService.Enumerations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.LessonManagement.LessonBase.Schemas
{
    public class LessonProgressUpdateRequest : IValidatableObject
    {
        [SwaggerIgnore]
        public Guid LessonId { get; set; }

        public int TimeSpent { get; set; }

        /// <summary>
        /// Text, Video, Quiz
        /// </summary>
        [EnumDataType(typeof(LessonType))]
        public LessonType LessonType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LessonType == LessonType.Video && TimeSpent <= 0)
            {
                yield return new ValidationResult("TimeSpent must be greater than 0", [nameof(TimeSpent)]);
            }

            if ((LessonType == LessonType.Quiz || LessonType == LessonType.Text) && TimeSpent > 0)
            {
                yield return new ValidationResult("TimeSpent must be 0", [nameof(TimeSpent)]);
            }
        }
    }
}