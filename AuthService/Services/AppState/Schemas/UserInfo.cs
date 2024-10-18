namespace AuthService.Services.AppState.Schemas
{
    public class UserInfo
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }
}