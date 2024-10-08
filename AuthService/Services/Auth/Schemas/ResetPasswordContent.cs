namespace AuthService.Services.Auth.Schemas
{
    public class ResetPasswordContent
    {
        public int UserId { get; set; }

        public string Token { get; set; }

        public string NewPassword { get; set; }
    }
}