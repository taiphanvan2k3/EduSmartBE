using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.DiscussionManagement.Comments.Schemas
{
    public class CommentCreateDto
    {
        public Guid? ParentId { get; set; }

        [Required]
        public string Content { get; set; }

        [SwaggerIgnore]
        public Guid DiscussionId { get; set; }
    }
}