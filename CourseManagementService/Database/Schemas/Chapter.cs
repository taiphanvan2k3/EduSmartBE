using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Database.Schemas
{
    public class Chapter : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }

        public Guid CourseId { get; set; }

        public virtual Course Course { get; set; }
    }
}