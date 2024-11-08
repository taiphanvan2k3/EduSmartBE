using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using Newtonsoft.Json;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class CourseSearchItem : ICourseWithTeacher
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailURL { get; set; }

        public int TotalStudents { get; set; }

        public int TotalLessons { get; set; }

        [JsonIgnore]
        public List<long> TotalSecondsPerChapter { get; set; }

        public long TotalSeconds
        {
            get
            {
                return TotalSecondsPerChapter.Sum();
            }
        }

        public TeacherDetail Teacher { get; set; }
    }
}