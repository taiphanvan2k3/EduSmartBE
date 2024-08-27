using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Databases.Schemas
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? AvatarURL { get; set; }

        public string? Role { get; set; }

        public string? Phone { get; set; }

        public bool IsOnline { get; set; }

        public bool IsActive { get; set; }
        
        public ICollection<UserLogin>? Logins { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }
    }
}