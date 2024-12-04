using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CourseManagementService.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // Dùng StringEnumConverter để tránh lỗi gửi request dạng string
    public enum NoteSearchType
    {
        [EnumMember(Value = "lesson")]
        Lesson = 1,

        [EnumMember(Value = "chapter")]
        Chapter = 2,

        [EnumMember(Value = "course")]
        Course = 3
    }
}