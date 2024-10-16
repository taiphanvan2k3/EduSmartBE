namespace CourseManagementService.Services.CourseManagement.Teacher.Schemas
{
    public class CourseDetailDto : CourseDto
    {
        public int TotalStudents { get; set; }

        public int TotalLessons { get; set; }

        public int TotalMinutes { get; set; }
    }
}