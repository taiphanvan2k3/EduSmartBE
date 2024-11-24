using System.ComponentModel.DataAnnotations;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;

namespace CourseManagementService.Services.LessonManagement.VideoLesson.Schemas
{
    public class VideoLessonCreateUpdateDto : TextLessonCreateUpdateDto, IValidatableObject
    {
        public IFormFile Video { get; set; }

        public IFormFile Thumbnail { get; set; }

        public int? VideoDurationInSeconds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Video != null)
            {
                // Check có phải video không
                if (Video.ContentType != "video/mp4")
                {
                    yield return new ValidationResult("File must be a video", [nameof(File)]);
                }
            }

            if (Thumbnail != null)
            {
                // Check có phải ảnh không
                if (Thumbnail.ContentType != "image/jpeg" && Thumbnail.ContentType != "image/png")
                {
                    yield return new ValidationResult("File must be an image", [nameof(Thumbnail)]);
                }
            }
        }
    }
}