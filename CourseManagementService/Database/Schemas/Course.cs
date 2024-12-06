using System.ComponentModel.DataAnnotations;
using CourseManagementService.Database.Schemas.DiscussionEntities;
using CourseManagementService.Enumerations;

namespace CourseManagementService.Database.Schemas
{
    public class Course : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> CoreValues { get; set; }

        /// <summary>
        /// Điều kiện tiên quyết
        /// </summary>
        public List<string> Prerequisites { get; set; }

        public string ThumbnailURL { get; set; }

        [MaxLength(300)]
        public string PreviewVideoURL { get; set; }

        public decimal Price { get; set; }

        public int CurrencyId { get; set; }

        public CourseType Type { get; set; }

        public int TeacherId { get; set; }

        public bool IsPublished { get; set; } = true;

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [Required]
        public Currency Currency { get; set; }

        public virtual ICollection<CourseTag> Tags { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }

        public virtual ICollection<LessonTracking> LessonTrackings { get; set; }

        public virtual ICollection<CourseEnrollment> Enrollments { get; set; }

        public virtual ICollection<CourseRating> Ratings { get; set; }

        public virtual ICollection<Discussion> Discussions { get; set; }

        public virtual ICollection<Bookmark> Bookmarks { get; set; }
    }
}