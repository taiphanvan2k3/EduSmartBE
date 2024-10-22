using CourseManagementService.Services.CourseManagement.Teacher.Schemas;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class ListOfSearchItems
    {
        public List<TeacherSearchItem> Teachers { get; set; }

        public List<CourseSearchItem> Courses { get; set; }

        public ListOfSearchItems()
        {
            Teachers = [];
            Courses = [];
        }
    }
}