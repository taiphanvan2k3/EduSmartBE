using CourseManagementService.Enumerations;

namespace CourseManagementService.Services.CourseManagement.Student.Schemas
{
    public class SingleUpdateVisibilityStatus
    {
        public CourseProgressVisibility VisibilityStatus { get; set; }
    }

    public class UpdateAllVisibilityStatus
    {
        public CourseProgressVisibility VisibilityStatus { get; set; }
    }
}