using System.ComponentModel.DataAnnotations;
using CourseManagementService.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    public class Note : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [Comment("Highlighted text from the lesson")]
        public string Content { get; set; }

        [Comment("Note about the highlighted text")]
        public string Comment { get; set; }

        public int UserId { get; set; }

        [Comment("The time of the lesson when the note was created")]
        public int TimeMilestone { get; set; }

        public Guid LessonId { get; set; }

        public Guid ChapterId { get; set; }

        public LessonType LessonType { get; set; }

        public bool IsDelFlag { get; set; }

        public virtual Lesson Lesson { get; set; }

        public virtual Chapter Chapter { get; set; }
    }
}