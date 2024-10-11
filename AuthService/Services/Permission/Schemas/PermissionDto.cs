using AuthService.Services.Permission.Schemas.Function;

namespace AuthService.Services.Permission.Schemas
{
    public class PermissionDto
    {
        public FunctionDto Function { get; set; }

        public bool IsActive { get; set; }
    }
}