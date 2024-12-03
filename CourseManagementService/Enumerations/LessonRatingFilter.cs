using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CourseManagementService.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // Dùng StringEnumConverter để tránh lỗi gửi request dạng string
    public enum LessonRatingFilter
    {
        [EnumMember(Value = "all")]
        All = 0,

        [EnumMember(Value = "like")]
        Like = 1,

        [EnumMember(Value = "dislike")]
        Dislike = 2
    }
}