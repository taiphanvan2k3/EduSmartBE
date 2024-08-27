using System.ComponentModel.DataAnnotations;

namespace AuthService.Databases.Schemas
{
    public class Screen
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public ICollection<Function> Functions { get; set; }
    }
}