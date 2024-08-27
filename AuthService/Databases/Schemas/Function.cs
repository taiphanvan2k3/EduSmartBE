using System.ComponentModel.DataAnnotations;

namespace AuthService.Databases.Schemas
{
    public class Function
    {
        [Key]
        public int Id { get; set; }

        public int ScreenId { get; set; }

        public string FunctionName { get; set; }

        public string Description { get; set; }

        public Screen Screen { get; set; }

        public ICollection<Permission> Permissions { get; set; }
    }
}