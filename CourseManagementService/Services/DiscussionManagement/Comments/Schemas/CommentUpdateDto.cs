using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.DiscussionManagement.Comments.Schemas
{
    public class CommentUpdateDto
    {
        [SwaggerIgnore]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }
    }
}