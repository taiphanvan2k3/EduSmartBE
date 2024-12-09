using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class UploadHistory
    {
        public Guid Id { get; set; }

        public long StorageAmount { get; set; }

        /// <summary>
        /// The resource type that the file is uploaded for.
        /// </summary>
        [Comment("The resource type that the file is uploaded for.")]
        public ResourceType ResourceType { get; set; }

        /// <summary>
        /// Id of the resource that the file is uploaded for. <br/>
        /// Such as CourseId, VideoLessonId, AttachmentId, CommentId...
        /// </summary>
        [Comment("CourseId, VideoLessonId, AttachmentId, CommentId...")]
        public Guid ResourceId { get; set; }

        public DateTimeOffset UploadDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(1000)]
        public string FilePath { get; set; }

        public FileType FileType { get; set; }

        public MediaStorageProvider MediaStorageProvider { get; set; }

        public int StorageInfoId { get; set; }

        public virtual StorageInfo StorageInfo { get; set; }
    }
}