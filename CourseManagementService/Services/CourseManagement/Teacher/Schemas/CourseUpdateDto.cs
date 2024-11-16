using System.ComponentModel.DataAnnotations;
using CourseManagementService.Enumerations;

namespace CourseManagementService.Services.CourseManagement.Teacher.Schemas
{
    public class CourseUpdateDto : IValidatableObject
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public List<string> CoreValues { get; set; }

        public List<string> Prerequisites { get; set; }

        public IFormFile Thumbnail { get; set; }

        public IFormFile PreviewVideo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [EnumDataType(typeof(CurrencyType), ErrorMessage = "Invalid currency type.")]
        public CurrencyType Currency { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [EnumDataType(typeof(CourseType), ErrorMessage = "Invalid course type.")]
        public CourseType Type { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public List<int> TagIds { get; set; }

        public bool IsPublished { get; set; }

        public CourseUpdateDto()
        {
            TagIds = [];
        }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (CategoryId == 0)
            {
                yield return new ValidationResult("CategoryId is required.", [nameof(CategoryId)]);
            }
        }
    }
}