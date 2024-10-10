using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Permission.Schemas
{
    public class PermissionStatus
    {
        public int RoleId { get; set; }

        [Required]
        public string FunctionId { get; set; }

        public bool IsActive { get; set; }
    }
}