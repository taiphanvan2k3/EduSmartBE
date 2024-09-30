namespace UserService.Services.Users.Schemas
{
    public class UserDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}