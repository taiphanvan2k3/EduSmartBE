using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagementService.Database.Schemas
{
    [Table("Currencies")]
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(3)]
        public string Code { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}