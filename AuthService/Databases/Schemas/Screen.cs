using System.ComponentModel.DataAnnotations;

namespace AuthService.Databases.Schemas
{
    public class Screen : BaseEntity
    {
        [MaxLength(20)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string Code { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        public int Order { get; set; }

        public ICollection<Function> Functions { get; }
    }
}