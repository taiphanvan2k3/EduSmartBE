using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas.DiscussionEntities
{
    public class Discussion : BaseEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsAnswered { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        /// <summary>
        /// Sẽ dùng cái này trong trường hợp owner muốn xoá discussion và đã có người comment vào discussion đó
        /// </summary>
        public bool IsDelFlag { get; set; }

        public int CreatedBy { get; set; }

        /// <summary>
        /// Role of the user who created the discussion
        /// </summary>
        [Comment("Role of the user who created the discussion")]
        [Required]
        public string RoleOfUser { get; set; }

        /// <summary>
        /// Id of the discussion type
        /// </summary>
        [Comment("Id of the discussion type")]
        public int TypeId { get; set; }

        public Guid LessonId { get; set; }

        /// <summary>
        /// Make the query faster when querying by course and userID
        /// </summary>
        public Guid CourseId { get; set; }

        public virtual DiscussionType Type { get; set; }

        public virtual Lesson Lesson { get; set; }

        public virtual Course Course { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}