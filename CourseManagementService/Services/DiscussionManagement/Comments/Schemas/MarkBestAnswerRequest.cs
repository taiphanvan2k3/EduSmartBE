using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.DiscussionManagement.Comments.Schemas
{
    public class MarkBestAnswerRequest
    {
        [SwaggerIgnore]
        public Guid DiscussionId { get; set; }

        public Guid CommentId { get; set; }

        public bool IsTurnOn { get; set; }
    }
}