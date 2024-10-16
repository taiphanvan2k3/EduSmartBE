using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Database.Schemas
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// The Tag created by the user will only show for that user except for the admin
        /// </summary>
        public int CreatedBy { get; set; }

        public bool IsCreatedByAdmin { get; set; }

        public virtual ICollection<CourseTag> Courses { get; set; }
    }
}