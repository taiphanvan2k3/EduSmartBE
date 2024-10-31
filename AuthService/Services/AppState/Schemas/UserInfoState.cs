namespace AuthService.Services.AppState.Schemas
{
    public class UserInfoState
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }
}