namespace CourseManagementService.Database.Schemas.NotificationEntities
{
    public class SenderInfo
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string AvatarURL { get; set; }

        public bool IsSystem { get; set; }

        public bool IsTeacher { get; set; }
    }
}