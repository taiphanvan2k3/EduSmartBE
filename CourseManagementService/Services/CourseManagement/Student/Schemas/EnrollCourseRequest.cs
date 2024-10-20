namespace CourseManagementService.Services.CourseManagement.Student.Schemas
{
    public class EnrollCourseRequest
    {
        public Guid CourseId { get; set; }

        public DateTimeOffset EnrollmentDate { get; set; }
    }
}