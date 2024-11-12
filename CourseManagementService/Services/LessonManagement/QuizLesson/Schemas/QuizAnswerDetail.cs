namespace CourseManagementService.Services.LessonManagement.QuizLesson.Schemas
{
    public class QuizAnswerDetail
    {
        public long Id { get; set; }

        public string Answer { get; set; }

        public bool IsCorrect { get; set; }

        public string Explanation { get; set; }
    }
}