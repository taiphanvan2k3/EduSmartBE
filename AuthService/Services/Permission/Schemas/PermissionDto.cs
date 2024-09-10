namespace AuthService.Services.Permission.Schemas
{
    public class PermissionDto
    {
        public string FunctionId { get; set; }

        public string FunctionName { get; set; }

        public bool IsActive { get; set; }
    }
}