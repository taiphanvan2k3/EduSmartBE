using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.DiscussionManagement.Comments.Schemas
{
    public class CommentCreateDto : IValidatableObject
    {
        public Guid? ParentId { get; set; }

        public int? MentionedUserId { get; set; }

        [Required]
        public string Content { get; set; }

        [SwaggerIgnore]
        public Guid DiscussionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ParentId.HasValue && MentionedUserId.HasValue)
            {
                yield return new ValidationResult("Invalid mentioned user id when creating a root comment", [nameof(MentionedUserId)]);
            }
        }
    }
}