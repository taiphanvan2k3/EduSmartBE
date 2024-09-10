using System.ComponentModel.DataAnnotations;

namespace AuthService.Databases.Schemas
{
    public class Function
    {
        [MaxLength(20)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string Code { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        public int Order { get; set; }

        [MaxLength(20)]
        public string ScreenId { get; set; }

        public Screen Screen { get; set; }

        public ICollection<Permission> Permissions { get; set; }

        public ICollection<CoursePermission> CoursePermissions { get; set; }
    }
}