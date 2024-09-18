using System.ComponentModel.DataAnnotations;

namespace UserService.Databases.Schemas
{
    public class User
    {
        [MaxLength(20)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime LastLogout { get; set; }

        public bool IsOnline { get; set; }

        public bool IsActive { get; set; }
    }
}