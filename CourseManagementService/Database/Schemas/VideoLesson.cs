using System.ComponentModel.DataAnnotations;
using CourseManagementService.Enumerations;

namespace CourseManagementService.Database.Schemas
{
    public class VideoLesson : BaseEntity
    {
        public Guid Id { get; set; }

        public string PublicVideoURL { get; set; }

        [Required]
        [MaxLength(200)]
        public string ThumbnailURL { get; set; }

        public UploadStatus UploadStatus { get; set; }

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; }
    }
}