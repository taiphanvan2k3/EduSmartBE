namespace AuthService.Services.Auth.Schemas
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }

        public string Code { get; set; }

        public string Role { get; set; }
    }
}