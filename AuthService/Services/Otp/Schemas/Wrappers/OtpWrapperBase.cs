namespace AuthService.Services.Otp.Schemas.Wrappers
{
    public class OtpWrapperBase
    {
        public string OtpCode { get; set; }

        public DateTimeOffset OtpExpiry { get; set; } = DateTimeOffset.UtcNow;
    }
}