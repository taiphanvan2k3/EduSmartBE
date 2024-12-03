using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class ListOfCourseRatings
    {
        public CourseRatingOverall CourseRatingOverall { get; set; }

        public PaginatedList<CourseRatingDetail> Ratings { get; set; }
    }
}