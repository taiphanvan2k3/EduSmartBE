namespace CourseManagementService.Services.AppState.Schemas
{
    public class UserInfoState
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public List<string> Roles { get; set; }

        public bool IsAdmin => Roles.Contains("Admin");

        public bool IsTeacher => Roles.Contains("Teacher");
    }
}