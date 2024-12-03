namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class CourseRatingOverall
    {
        public Guid CourseId { get; set; }

        public double OverallRating { get; set; }

        public int TotalRatings { get; set; }
    }
}