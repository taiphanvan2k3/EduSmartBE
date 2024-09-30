using System.ComponentModel.DataAnnotations;

namespace UserService.Databases.Schemas
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}