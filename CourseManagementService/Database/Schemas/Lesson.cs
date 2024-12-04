using System.ComponentModel.DataAnnotations;
using CourseManagementService.Database.Schemas.DiscussionEntities;
using CourseManagementService.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    public class Lesson : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Comment("The text content of the lesson")]
        public string Description { get; set; }

        public Guid ChapterId { get; set; }

        public Chapter Chapter { get; set; }

        public long DurationInSeconds { get; set; }

        public LessonType LessonType { get; set; }

        public long CreatedBy { get; set; }

        [Comment("Maybe the lesson is not ready to be published")]
        public bool IsPublished { get; set; }

        public DateTimeOffset? PublishedAt { get; set; }

        public bool IsCommentAllowed { get; set; } = true;

        public bool IsRatingAllowed { get; set; } = true;

        public int Order { get; set; }

        public DifficultyLevel DifficultyLevel { get; set; }

        [Comment("This lesson is a quiz lesson")]
        public virtual QuizLesson QuizLesson { get; set; }

        [Comment("This lesson is a video lesson")]
        public virtual VideoLesson VideoLesson { get; set; }

        public virtual ICollection<LessonRating> Ratings { get; set; }

        public virtual ICollection<LessonTracking> LessonTrackings { get; set; }

        public virtual ICollection<Discussion> Discussions { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<Bookmark> Bookmarks { get; set; }
    }
}