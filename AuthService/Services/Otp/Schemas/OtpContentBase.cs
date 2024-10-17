using System.ComponentModel.DataAnnotations;
using AuthService.Enumerations;

namespace AuthService.Services.Otp.Schemas
{
    /// <summary>
    /// This class is used for from body of request
    /// </summary>
    public class OtpContentBase
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }

        [EnumDataType(typeof(OtpType), ErrorMessage = "Invalid OtpType")]
        public OtpType OtpType { get; set; }
    }
}