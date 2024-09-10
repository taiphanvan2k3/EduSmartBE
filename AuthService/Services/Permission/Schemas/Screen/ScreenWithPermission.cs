namespace AuthService.Services.Permission.Schemas.Screen
{
    public class ScreenWithPermission : ScreenDto
    {
        public List<PermissionDto> Permissions { get; set; }
    }
}