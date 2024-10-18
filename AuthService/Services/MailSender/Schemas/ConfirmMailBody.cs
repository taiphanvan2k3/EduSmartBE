namespace AuthService.Services.MailSender.Schemas
{
    public class ConfirmMailBody : MailBodyBase
    {
        public string ConfirmLink { get; set; }
    }
}