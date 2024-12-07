using System.ComponentModel.DataAnnotations;
using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class SearchCondition : ParamsSearch, IValidatableObject
    {
        public string Keyword { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        public SearchCondition()
        {
            CurrentPage = 1;
            PageSize = 5;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Keyword))
            {
                yield return new ValidationResult("Keyword is required", [nameof(Keyword)]);
            }

            if (Keyword.Length < 3)
            {
                yield return new ValidationResult("Keyword must be at least 3 characters", [nameof(Keyword)]);
            }
        }
    }
}