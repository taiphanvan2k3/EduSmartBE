namespace CourseManagementService.Common.Schemas
{
    public class ParamsSearch
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public ParamsSearch()
        {
            CurrentPage = 1;
            PageSize = 10;
        }
    }
}