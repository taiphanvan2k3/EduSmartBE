using CourseManagementService.Enumerations;

namespace CourseManagementService.Database.Schemas.NotificationEntities
{
    public class UserNotification
    {
        public Guid Id { get; set; }

        public int ReceiverId { get; set; }

        public SenderInfo SenderInfo { get; set; }

        public NotificationType Type { get; set; }

        public string RelatedEntityId { get; set; }

        public RelatedEntityType RelatedEntityType { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Guid CourseId { get; set; }

        public virtual Course Course { get; set; }

        public bool IsRead { get; set; }

        /// <summary>
        /// <b>Metadata format depends on the notification type:</b><br/>
        /// 
        /// <b>For reaction notifications:</b>
        /// <list type="bullet">
        ///   <item><description><c>ReactionType</c>: The type of reaction (e.g., like, love, etc.)</description></item>
        ///   <item><description><c>ReactionCount</c>: The number of reactions (excluding the current user's reaction)</description></item>
        ///   <item><description><c>Discussion Id</c>: Id of discussion</description></item>
        /// </list>
        /// 
        /// <b>For new lesson notifications:</b>
        /// <list type="bullet">
        ///   <item><description><c>LessonTitle</c>: The title of the lesson</description></item>
        ///   <item><description><c>CourseName</c>: The name of the course</description></item>
        /// </list>
        /// </summary>
        public string MetaData { get; set; }
    }
}