using System.ComponentModel.DataAnnotations;

namespace AuthService.Databases.Schemas
{
    public class Permission
    {
        public int RoleId { get; set; }

        [MaxLength(20)]
        public string FunctionId { get; set; }

        public Function Function { get; set; }
    }
}