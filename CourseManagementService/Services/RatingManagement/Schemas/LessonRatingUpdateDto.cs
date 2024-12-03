using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class LessonRatingUpdateDto : LessonRatingCreateDto
    {
        [SwaggerIgnore]
        public Guid Id { get; set; }
    }
}