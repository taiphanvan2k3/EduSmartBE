namespace AuthService.Services.Auth.Schemas
{
    public class OtpData
    {
        public string OtpCode { get; set; }

        public string ResetPasswordToken { get; set; }
    }
}