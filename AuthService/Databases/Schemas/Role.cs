using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Databases.Schemas
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }
        
        public ICollection<Permission>? Permissions { get; set; }
    }
}