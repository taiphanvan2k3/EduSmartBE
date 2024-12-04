using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CourseManagementService.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // Dùng StringEnumConverter để tránh lỗi gửi request dạng string
    public enum DateOrder
    {
        [EnumMember(Value = "oldest")]
        Oldest = 1,

        [EnumMember(Value = "latest")]
        Latest = 2
    }
}