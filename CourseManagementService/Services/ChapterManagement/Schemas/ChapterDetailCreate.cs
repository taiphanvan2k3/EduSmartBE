namespace CourseManagementService.Services.ChapterManagement.Schemas
{
    public class ChapterDetailCreate
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string CourseId { get; set; }
    }
}