using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Services.ChapterManagement.Schemas
{
    public class ChapterDetailCreate
    {
        [Required]
        public string Name { get; set; }

        public Guid CourseId { get; set; }

        public bool IsPublished { get; set; }
    }
}