using CourseManagementService.Services.ChapterManagement.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using Newtonsoft.Json;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class CourseDetail : ICourseWithTeacher
    {
        [JsonProperty(Order = -1)]
        public TeacherDetail Teacher { get; set; }

        [JsonProperty(Order = 0)]
        public CourseDto Course { get; set; }

        [JsonProperty(Order = 100)]
        public List<ChapterDetail> Chapters { get; set; }

        [JsonProperty(Order = 101)]
        public List<LessonTrackingDetail> LearnedLessons { get; set; } = [];
    }
}