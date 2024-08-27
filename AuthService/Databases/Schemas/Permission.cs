using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Databases.Schemas
{
    public class Permission
    {
        public int RoleId { get; set; }

        public int FunctionId { get; set; }

        public Role? Role { get; set; }
        public Function? Function { get; set; }
    }
}