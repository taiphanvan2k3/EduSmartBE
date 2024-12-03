namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class LessonRatingOverall
    {
        public Guid LessonId { get; set; }

        public string LessonName { get; set; }

        public int LikeCount { get; set; }

        public int DislikeCount { get; set; }
    }
}