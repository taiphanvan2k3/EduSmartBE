using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    public class CourseRating : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Course Course { get; set; }

        [Comment("This user maybe a student or a teacher")]
        public long UserId { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        [MaxLength(200)]
        public string Comment { get; set; }
    }
}