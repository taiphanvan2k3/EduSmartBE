using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Auth.Schemas
{
    public class OtpCodeContent
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }
    }

    public class ResetPasswordContent
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}