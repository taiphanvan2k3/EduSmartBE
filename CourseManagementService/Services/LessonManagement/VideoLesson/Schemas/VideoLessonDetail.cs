using CourseManagementService.Common;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using Newtonsoft.Json;

namespace CourseManagementService.Services.LessonManagement.VideoLesson.Schemas
{
    public class VideoLessonDetail : LessonDetail
    {
        [JsonProperty(Order = 5)]
        public string BaseBlobURL { get; set; }

        [JsonProperty(Order = 5)]
        public string VideoURLWithSAS { get; set; }

        [JsonProperty(Order = 6)]
        public string ThumbnailURL { get; set; }

        [JsonProperty(Order = 7)]
        public LookupDto UploadStatus { get; set; }
    }
}