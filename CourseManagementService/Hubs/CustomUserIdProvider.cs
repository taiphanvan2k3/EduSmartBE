using Microsoft.AspNetCore.SignalR;

namespace CourseManagementService.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Lấy UserId từ JWT Token
            return connection.User?.FindFirst("userId")?.Value;
        }
    }
}