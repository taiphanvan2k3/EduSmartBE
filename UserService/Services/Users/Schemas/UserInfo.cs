namespace UserService.Services.Users.Schemas
{
    public class UserInfo
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarURL { get; set; }

        public string Phone { get; set; }

        public int Gender { get; set; }
    }
}