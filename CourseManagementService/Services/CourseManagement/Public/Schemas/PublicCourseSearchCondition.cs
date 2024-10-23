using CourseManagementService.Common.Schemas;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class PublicCourseSearchCondition : ParamsSearchWithSingleSort
    {
        public string Keyword { get; set; } = string.Empty;

        [SwaggerIgnore]
        public override string OrderBy => string.IsNullOrEmpty(SortBy) ? "Id" : SortBy switch
        {
            "Newest" => "UpdatedAt",
            "Popularity" => "Enrollments.Count",
            "Price" => "Price",
            _ => throw new Exception("Invalid sort by parameter")
        };
    }
}