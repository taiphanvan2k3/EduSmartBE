using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.CourseManagement.Teacher.Schemas
{
    public class CourseSearchCondition : ParamsSearch
    {
        public string Keyword { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
    }
}