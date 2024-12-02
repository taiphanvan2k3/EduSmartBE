using CourseManagementService.Common.Schemas;
using Newtonsoft.Json;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class DiscussionDetail : DiscussionInfo
    {
        [JsonProperty(Order = 5)]
        public UserDetail User { get; set; }

        [JsonProperty(Order = 6)]
        public string Content { get; set; }

        [JsonProperty(Order = 7)]
        public Guid LessonId { get; set; }

        [JsonProperty(Order = 9)]
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}