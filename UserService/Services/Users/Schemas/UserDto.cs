namespace UserService.Services.Users.Schemas
{
    public class UserDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime LastLogout { get; set; }

        public bool IsOnline { get; set; }

        public bool IsActive { get; set; }
    }
}