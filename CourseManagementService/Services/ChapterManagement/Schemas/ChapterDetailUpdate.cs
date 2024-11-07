using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.ChapterManagement.Schemas
{
    public class ChapterDetailUpdate : IValidatableObject
    {
        [SwaggerIgnore]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Order { get; set; }

        public bool IsPublished { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Order <= 0)
            {
                yield return new ValidationResult("Order must be greater than 0", [nameof(Order)]);
            }
        }
    }
}