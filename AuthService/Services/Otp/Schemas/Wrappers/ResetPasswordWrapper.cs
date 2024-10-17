namespace AuthService.Services.Otp.Schemas.Wrappers
{
    public class ResetPasswordWrapper : OtpWrapperBase
    {
        public string ResetPasswordToken { get; set; }
    }
}