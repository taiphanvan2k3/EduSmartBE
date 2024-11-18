namespace CourseManagementService.Enumerations
{
    public enum SupportRequestStatus
    {
        Pending = 1,  // Đang chờ giải quyết
        Resolved = 2,  // Đã giải quyết
        Closed = 3  // Đã đóng (có thể là khi không cần hỗ trợ nữa)
    }
}