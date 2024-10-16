using AuthService.AsyncDataServices;
using AuthService.BackgroundServices;
using AuthService.Databases.Schemas;
using AuthService.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.Services.Auth.Schemas
{
    public class AuthServiceDependencies
    {
        public UserManager<ApplicationUser> UserManager { get; set; }
        public SignInManager<ApplicationUser> SignInManager { get; set; }
        public IUrlHelper UrlHelper { get; set; }
        public IOptions<ServerSetting> ServerSetting { get; set; }
        public MailProducer MailProducer { get; set; }
        public CommonProducer CommonProducer { get; set; }
        public ITokenService TokenService { get; set; }
        public IGoogleAuthService GoogleAuthService { get; set; }
        public IMessagePublisher MessageBusPublisher { get; set; }
    }

}