using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using Newtonsoft.Json;

namespace CourseManagementService.Services.LessonManagement.QuizLesson.Schemas
{
    public class QuizLessonDetail : LessonDetail
    {
        [JsonProperty(Order = 6)]
        public string Question { get; set; }

        [JsonProperty(Order = 7)]
        public List<QuizAnswerDetail> Answers { get; set; }

        [JsonProperty(Order = 8)]
        public bool IsMultipleChoice { get; set; }
    }
}