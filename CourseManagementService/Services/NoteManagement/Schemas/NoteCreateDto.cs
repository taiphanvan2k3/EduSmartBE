using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.NoteManagement.Schemas
{
    public class NoteCreateDto
    {
        [Required]
        public string Content { get; set; }

        public string Comment { get; set; }

        [SwaggerIgnore]
        public Guid LessonId { get; set; }

        public int TimeMilestone { get; set; }
    }
}