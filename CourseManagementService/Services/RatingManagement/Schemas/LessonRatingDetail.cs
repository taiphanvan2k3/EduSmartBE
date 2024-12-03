using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.RatingManagement.Schemas
{
    public class LessonRatingDetail
    {
        public Guid Id { get; set; }

        public string Comment { get; set; }

        public bool IsLike { get; set; }

        public UserDetail User { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}