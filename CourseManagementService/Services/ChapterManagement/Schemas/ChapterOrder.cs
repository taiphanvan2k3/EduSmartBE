using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;

namespace CourseManagementService.Services.ChapterManagement.Schemas
{
    public class ChapterOrder
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public List<LessonOrder> LessonOrders { get; set; } = [];
    }
}