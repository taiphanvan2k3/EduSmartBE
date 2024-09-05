namespace AuthService.Services.MailSender.Schemas
{
    public class ConfirmMailBody : MailBodyBase
    {
        public string Content { get; set; }

        public string ConfirmLink { get; set; }
    }
}