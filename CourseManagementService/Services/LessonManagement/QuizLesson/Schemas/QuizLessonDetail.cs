using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using Newtonsoft.Json;

namespace CourseManagementService.Services.LessonManagement.QuizLesson.Schemas
{
    public class QuizLessonDetail : LessonDetail
    {
        [JsonProperty(Order = 9)]
        public string Question { get; set; }

        [JsonProperty(Order = 10)]
        public List<QuizAnswerDetail> Answers { get; set; }

        [JsonProperty(Order = 11)]
        public bool IsMultipleChoice { get; set; }
    }
}