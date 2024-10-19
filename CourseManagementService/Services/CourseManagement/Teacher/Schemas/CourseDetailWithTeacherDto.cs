namespace CourseManagementService.Services.CourseManagement.Teacher.Schemas
{
    public class CourseDetailWithTeacherDto : CourseDto
    {
        public TeacherDetail Teacher { get; set; }
    }
}