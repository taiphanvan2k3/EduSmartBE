using System.Text.Json.Serialization;
using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class SearchCondition : ParamsSearch
    {
        public TargetUserType TargetUserType { get; set; }

        public SearchCondition()
        {
            PageSize = 8;
        }
    }


    [JsonConverter(typeof(JsonStringEnumConverter))] // Cho phép Swagger sử dụng Enum string thay vì int
    public enum TargetUserType
    {
        OtherUsers = 1,
        Me = 2
    }
}