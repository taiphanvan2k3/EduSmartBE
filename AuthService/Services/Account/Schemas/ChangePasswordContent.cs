namespace AuthService.Services.Account.Schemas
{
    public class ChangePasswordContent
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}