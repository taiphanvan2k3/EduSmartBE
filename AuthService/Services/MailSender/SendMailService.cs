using AuthService.Services.MailSender.Schemas;
using AuthService.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AuthService.Services.MailSender
{
    public interface ISendMailService
    {
        public Task SendMailConfirmAccount(ConfirmMailBody mailBody);

        public Task SendEmailResetPassword(ResetPasswordMailBody mailBody);
    }

    public class SendMailService(IOptions<MailSetting> mailSettings,
        RazorViewService razorViewService,
        ILogger<SendMailService> logger) : ISendMailService
    {
        private readonly MailSetting _mailSettings = mailSettings?.Value
            ?? throw new ArgumentNullException(nameof(mailSettings));

        private readonly ILogger<SendMailService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        public async Task SendMailConfirmAccount(ConfirmMailBody mailBody)
        {
            try
            {
                _logger.LogInformation("Send mail confirm account to {ToEmail}", mailBody.ToEmail);
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                emailMessage.To.Add(new MailboxAddress(mailBody.ToUserName, mailBody.ToEmail));
                emailMessage.Subject = mailBody.Subject;

                string content = await razorViewService.RenderViewToStringAsync("Views/Templates/ConfirmAccount.cshtml", mailBody);
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = content
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, useSsl: false); // nếu dùng ssl thì port là 465
                await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Send mail confirm account to {ToEmail} successfully", mailBody.ToEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send mail confirm account to {ToEmail} FAILED", mailBody.ToEmail);
            }
        }


        public async Task SendEmailResetPassword(ResetPasswordMailBody mailBody)
        {
            try
            {
                _logger.LogInformation("Send mail reset password to {ToEmail}", mailBody.ToEmail);
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                emailMessage.To.Add(new MailboxAddress(mailBody.ToUserName, mailBody.ToEmail));
                emailMessage.Subject = mailBody.Subject;

                string content = await razorViewService.RenderViewToStringAsync("Views/Templates/ResetPassword.cshtml", mailBody);
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = content
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, useSsl: false); // nếu dùng ssl thì port là 465
                await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Send mail reset password to {ToEmail} successfully", mailBody.ToEmail);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Send mail reset password to {ToEmail} FAILED", mailBody.ToEmail);
                throw;
            }
        }
    }
}