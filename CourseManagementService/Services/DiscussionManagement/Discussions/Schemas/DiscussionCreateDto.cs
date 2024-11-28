using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class DiscussionCreateDto : IValidatableObject
    {
        public Guid LessonId { get; set; }

        public int TypeId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TypeId <= 0)
            {
                yield return new ValidationResult("Discussion type is invalid");
            }
        }
    }
}