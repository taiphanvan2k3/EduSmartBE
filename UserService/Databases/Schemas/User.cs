using System.ComponentModel.DataAnnotations;

namespace UserService.Databases.Schemas
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(100)]
        [Required]
        public string Email { get; set; }

        public bool IsOnline { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastLogin { get; set; }

        public DateTimeOffset? LastLogout { get; set; }

        public virtual UserInfo UserInfo { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}