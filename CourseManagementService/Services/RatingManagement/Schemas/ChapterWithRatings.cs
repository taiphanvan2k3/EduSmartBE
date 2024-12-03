namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class ChapterWithRatings
    {
        public int Order { get; set; }

        public string Chapter { get; set; }

        public List<LessonRatingOverall> LessonRatings { get; set; }
    }
}