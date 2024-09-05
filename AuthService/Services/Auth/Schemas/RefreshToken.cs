namespace AuthService.Services.Auth.Schemas
{
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }
    }
}