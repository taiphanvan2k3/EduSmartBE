namespace AuthService.Services.Auth.Schemas
{
    public class AuthTokens
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public int AccessTokenExpireIn { get; set; }
    }
}