using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class CourseRatingDetail
    {
        public Guid Id { get; set; }

        public double Rating { get; set; }

        public string Comment { get; set; }

        public UserDetail User { get; set; }
    }
}