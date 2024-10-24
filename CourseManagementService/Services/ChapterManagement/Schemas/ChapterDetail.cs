namespace CourseManagementService.Services.ChapterManagement.Schemas
{
    public class ChapterDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string CourseId { get; set; }

    }
}