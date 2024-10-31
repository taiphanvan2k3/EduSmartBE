namespace AuthService.Services.Account.Schemas
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarURL { get; set; }

        public string Phone { get; set; }

        public int? Gender { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; }
    }
}