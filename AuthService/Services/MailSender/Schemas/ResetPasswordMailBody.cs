namespace AuthService.Services.MailSender.Schemas
{
    public class ResetPasswordMailBody : MailBodyBase
    {
        public string Content { get; set; }

        public string OtpCode { get; set; }
    }
}