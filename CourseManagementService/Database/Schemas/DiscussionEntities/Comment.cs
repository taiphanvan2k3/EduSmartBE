using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas.DiscussionEntities
{
    public class Comment : BaseEntity
    {
        public Guid Id { get; set; }

        public int CreatedBy { get; set; }

        public int? MentionedUserId { get; set; }

        public int VotersCount { get; set; }

        public Guid? ParentId { get; set; }

        public Comment Parent { get; set; }

        public bool IsApproved { get; set; }

        public Guid DiscussionId { get; set; }

        /// <summary>
        /// Role of the user who created the comment
        /// </summary>
        [Comment("Role of the user who created the comment")]
        [Required]
        public string RoleOfUser { get; set; }

        public CommentType Type { get; set; }

        public Discussion Discussion { get; set; }

        public virtual ICollection<Comment> Replies { get; set; }

        public virtual ICollection<Reaction> Reactions { get; set; }
    }
}