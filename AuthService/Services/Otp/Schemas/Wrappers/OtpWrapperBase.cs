using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Services.Otp.Schemas.Wrappers
{
    public class OtpWrapperBase
    {
        public string OtpCode { get; set; }

        [SwaggerIgnore]
        public DateTimeOffset OtpExpiry { get; set; } = DateTimeOffset.UtcNow;
    }
}