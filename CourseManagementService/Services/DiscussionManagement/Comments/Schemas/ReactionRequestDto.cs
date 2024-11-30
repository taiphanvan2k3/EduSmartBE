using System.ComponentModel.DataAnnotations;
using CourseManagementService.Enumerations;

namespace CourseManagementService.Services.DiscussionManagement.Comments.Schemas
{
    public class ReactionRequestDto
    {
        public bool IsTurnOn { get; set; }

        [EnumDataType(typeof(ReactionType))]
        public ReactionType Type { get; set; }
    }
}