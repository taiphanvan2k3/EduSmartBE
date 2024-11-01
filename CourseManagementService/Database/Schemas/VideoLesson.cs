using System.ComponentModel.DataAnnotations;
using CourseManagementService.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    public class VideoLesson : BaseEntity
    {
        public Guid Id { get; set; }

        [Comment("URL without SAS token")]
        [Required]
        public string BaseBlobURL { get; set; }

        [MaxLength(200)]
        public string ThumbnailURL { get; set; }

        public UploadStatus UploadStatus { get; set; }

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; }
    }
}