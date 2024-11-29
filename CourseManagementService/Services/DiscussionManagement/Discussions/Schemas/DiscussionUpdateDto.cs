using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class DiscussionUpdateDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}