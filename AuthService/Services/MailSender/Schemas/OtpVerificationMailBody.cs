namespace AuthService.Services.MailSender.Schemas
{
    public class OtpVerificationMailBody : MailBodyBase
    {
        public string OtpCode { get; set; }

        public string ActionName { get; set; }
    }

    public static class OtpVerificationType
    {
        public const string ResetPassword = "ResetPassword";

        public const string ConfirmDeleteAccount = "ConfirmDeleteAccount";
    }
}