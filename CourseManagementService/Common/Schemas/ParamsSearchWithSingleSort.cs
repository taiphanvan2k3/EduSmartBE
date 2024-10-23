using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Common.Schemas
{
    public class ParamsSearchWithSingleSort : ParamsSearch
    {
        public string SortBy { get; set; } = string.Empty;

        public string SortDirection { get; set; } = string.Empty;

        [SwaggerIgnore]
        public virtual string OrderBy
        {
            get
            {
                return string.IsNullOrEmpty(SortBy) ? "Id" : SortBy;
            }
        }

        [SwaggerIgnore]
        public string OrderDirection
        {
            get
            {
                return string.IsNullOrEmpty(SortDirection) ? "ASC" : SortDirection;
            }
        }
    }
}