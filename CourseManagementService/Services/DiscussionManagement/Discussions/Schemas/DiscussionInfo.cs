using CourseManagementService.Common;
using Newtonsoft.Json;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class DiscussionInfo
    {
        [JsonProperty(Order = 1)]
        public Guid Id { get; set; }

        [JsonProperty(Order = 2)]
        public string Title { get; set; }

        [JsonProperty(Order = 3)]
        public bool IsAnswered { get; set; }

        [JsonProperty(Order = 4)]
        public LookupDto Type { get; set; }

        [JsonProperty(Order = 5)]
        public bool IsDelFlag { get; set; }

        [JsonProperty(Order = 8)]
        public DateTimeOffset CreatedAt { get; set; }
    }
}