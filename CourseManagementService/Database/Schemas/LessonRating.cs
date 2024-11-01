using System.ComponentModel.DataAnnotations;

namespace CourseManagementService.Database.Schemas
{
    public class LessonRating : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid LessonId { get; set; }

        public Lesson Lesson { get; set; }

        public long StudentId { get; set; }

        public bool IsLike { get; set; }

        [MaxLength(200)]
        public string Comment { get; set; }
    }
}