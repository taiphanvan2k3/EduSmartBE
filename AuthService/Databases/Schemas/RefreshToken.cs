using System.ComponentModel.DataAnnotations;

namespace AuthService.Databases.Schemas
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Token { get; set; }

        [MaxLength(40)]
        public string IPAddress { get; set; }

        public int UserId { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime Expires { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public bool IsRevoked { get; set; }

        public bool IsActive => !IsRevoked && DateTime.UtcNow <= Expires;
    }
}