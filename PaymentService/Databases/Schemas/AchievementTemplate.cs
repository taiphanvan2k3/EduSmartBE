using System.ComponentModel.DataAnnotations;

namespace PaymentService.Databases.Schemas
{
    public class AchievementTemplate
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        [Required]
        public string Name { get; set; }

        [MaxLength(200)]
        [Required]
        public string ThumbnailURL { get; set; }

        [MaxLength(200)]
        [Required]
        public string TemplateURL { get; set; }

        public virtual ICollection<CourseAchievementTemplate> CourseAchievementTemplates { get; set; }
    }
}