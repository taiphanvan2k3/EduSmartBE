using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Common.Schemas
{
    public class ParamsSearch
    {
        [FromQuery(Name = "currentPage")] // tên query param trên URL
        public int CurrentPage { get; set; }

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; }

        public ParamsSearch()
        {
            CurrentPage = 1;
            PageSize = 10;
        }
    }
}