using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.ChapterManagement.Schemas
{
    public class ChapterDetailUpdate
    {
        [SwaggerIgnore]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsPublished { get; set; }
    }
}