using System.ComponentModel.DataAnnotations;
using CourseManagementService.Enumerations;

namespace CourseManagementService.Database.Schemas
{
    public class Course
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string BriefDescription { get; set; }

        public string DetailedDescription { get; set; }

        public string ThumbnailURL { get; set; }

        public decimal Price { get; set; }

        public CourseType Type { get; set; }

        public int TeacherId { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<CourseTag> Tags { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }
    }
}