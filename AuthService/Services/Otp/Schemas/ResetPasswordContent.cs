using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Otp.Schemas
{
    /// <summary>
    /// This class is used for from body of request
    /// </summary>
    public class ResetPasswordContent : OtpContentBase
    {
        [Required]
        public string NewPassword { get; set; }
    }
}