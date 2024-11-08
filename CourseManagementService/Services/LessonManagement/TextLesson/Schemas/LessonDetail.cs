using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using Newtonsoft.Json;

namespace CourseManagementService.Services.LessonManagement.TextLesson.Schemas
{
    public class LessonDetail
    {
        [JsonProperty(Order = 1)]
        public Guid Id { get; set; }

        [JsonProperty(Order = 2)]
        public string Title { get; set; }

        [JsonProperty(Order = 2)]
        public Guid ChapterId { get; set; }

        [JsonProperty(Order = 3)]
        public string Description { get; set; }

        [JsonProperty(Order = 4)]
        public LookupDto LessonType { get; set; }

        [JsonIgnore]
        public long DurationInSeconds { get; set; }

        [JsonProperty(Order = 7)]
        public string Duration
        {
            get
            {
                return Utils.ConvertSecondsToDuration(DurationInSeconds);
            }
        }

        [JsonProperty(Order = 10)]
        public bool IsPublished { get; set; }

        [JsonProperty(Order = 11)]
        public bool IsCommentAllowed { get; set; }

        [JsonProperty(Order = 12)]
        public bool IsRatingAllowed { get; set; }
    }
}