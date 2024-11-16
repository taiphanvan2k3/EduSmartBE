using CourseManagementService.Services.ChapterManagement.Schemas;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class CourseOrderSetting
    {
        public Guid CourseId { get; set; }

        public List<ChapterOrder> ChapterOrders { get; set; }
    }
}