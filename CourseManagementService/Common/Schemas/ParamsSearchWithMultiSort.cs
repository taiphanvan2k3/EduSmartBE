using Swashbuckle.AspNetCore.Annotations;

namespace CourseManagementService.Common.Schemas
{
    public class ParamsSearchWithMultiSort : ParamsSearch
    {
        public string SortBy { get; set; } = string.Empty;

        public string SortDirections { get; set; } = string.Empty;

        [SwaggerIgnore]
        public virtual List<string> OrderByList
        {
            get
            {
                return string.IsNullOrEmpty(SortBy)
                    ? ["Id"]
                    : SortBy.Split(',').Select(x => x.Trim()).ToList();
            }
        }

        [SwaggerIgnore]
        public virtual List<string> SortDirectionsList
        {
            get
            {
                return string.IsNullOrEmpty(SortDirections)
                    ? ["ASC"]
                    : SortDirections.Split(',').Select(x => x.Trim()).ToList();
            }
        }
    }
}