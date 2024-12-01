using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class CourseRatingUpdateDto : CourseRatingCreateDto
    {
        [SwaggerIgnore]
        public Guid Id { get; set; }
    }
}