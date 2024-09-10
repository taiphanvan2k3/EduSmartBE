namespace AuthService.Services.Permission.Schemas
{
    public class PermissionStatus
    {
        public int RoleId { get; set; }

        public string FunctionId { get; set; }

        public bool IsActive { get; set; }
    }
}