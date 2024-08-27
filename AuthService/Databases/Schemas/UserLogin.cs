using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Databases.Schemas
{
    public class UserLogin
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Provider { get; set; }

        public string? ProviderKey { get; set; }

        public string? PasswordHash { get; set; }

        public string? Salt { get; set; }

        public User? User { get; set; }
    }
}