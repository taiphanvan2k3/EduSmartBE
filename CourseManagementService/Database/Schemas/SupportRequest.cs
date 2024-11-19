using CourseManagementService.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.Schemas
{
    public class SupportRequest : BaseEntity
    {
        public Guid Id { get; set; }

        [Comment("UserId proceed the request")]
        public int FromUserId { get; set; }

        [Comment("UserId to proceed the request, such as TeacherId, AdminId")]
        public int ToUserId { get; set; }

        public string Description { get; set; }

        public string Response { get; set; }

        public DateTimeOffset? ResolvedAt { get; set; }

        public SupportRequestStatus Status { get; set; }

        public SupportRequestType Type { get; set; }

        // Ví dụ như vấn đề nội dung khoá học, sau khi giải quyết, có thể cần public để mọi người cùng tham khảo
        public bool IsPublic { get; set; }
    }
}