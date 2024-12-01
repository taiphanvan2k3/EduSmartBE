using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class CourseRatingCreateDto
    {
        [Range(0, 5)]
        public double Rating { get; set; }

        [SwaggerIgnore]
        public Guid CourseId { get; set; }

        [MaxLength(200)]
        [Required]
        public string Comment { get; set; }
    }
}