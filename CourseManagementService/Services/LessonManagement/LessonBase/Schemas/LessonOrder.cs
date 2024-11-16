namespace CourseManagementService.Services.LessonManagement.LessonBase.Schemas
{
    public class LessonOrder
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public Guid ChapterId { get; set; }
    }
}