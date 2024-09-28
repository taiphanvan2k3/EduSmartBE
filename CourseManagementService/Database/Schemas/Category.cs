using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Database.Schemas
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// The Category created by the user will only show for that user except for the admin
        /// </summary>
        [Required]
        public int CreatedBy { get; set; }

        public bool IsCreatedByAdmin { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}