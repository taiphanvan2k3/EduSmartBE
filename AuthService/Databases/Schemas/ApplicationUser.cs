using Microsoft.AspNetCore.Identity;

namespace AuthService.Databases.Schemas
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarURL { get; set; }

        public string Phone { get; set; }

        public bool IsOnline { get; set; }

        public bool IsActive { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }
    }
}