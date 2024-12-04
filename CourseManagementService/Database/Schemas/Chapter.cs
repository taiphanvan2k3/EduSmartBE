using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Database.Schemas
{
    public class Chapter : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int Order { get; set; }

        public bool IsPublished { get; set; }

        public Guid CourseId { get; set; }

        public virtual Course Course { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    }
}