using System.ComponentModel.DataAnnotations;
using AuthService.Commons;
using AuthService.CustomAttributes;
using AuthService.Enumerations;

namespace AuthService.Services.Auth.Schemas
{
    public class ExternalLoginRequest
    {
        [Required]
        [ProviderValidation(ProviderType.Google, ProviderType.Facebook, ProviderType.GitHub)]
        public string Provider { get; set; }

        [Required]
        public ExternalUserInfo UserInfo { get; set; }
    }

    public class ExternalUserInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Picture { get; set; }
    }
}