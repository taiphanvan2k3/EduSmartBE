namespace AuthService.Services.Auth.Schemas
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }
    }
}