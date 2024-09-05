namespace AuthService.Services.MailSender.Schemas
{
    public class MailBodyBase
    {
        public string Subject { get; set; }

        public string ToUserName { get; set; }

        public string ToEmail { get; set; }
    }
}