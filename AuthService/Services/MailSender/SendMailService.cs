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

        public Task SendEmailResetPassword(OtpVerificationMailBody mailBody);

        public Task SendEmailConfirmDeleteAccount(OtpVerificationMailBody mailBody);
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
                await SendMail(actionName: "SendMailConfirmAccount", templatePath: "Views/Templates/ConfirmAccount.cshtml", mailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send mail confirm account to {ToEmail} FAILED", mailBody.ToEmail);
                throw;
            }
        }


        public async Task SendEmailResetPassword(OtpVerificationMailBody mailBody)
        {
            try
            {
                await SendMail(actionName: "SendEmailResetPassword", templatePath: "Views/Templates/OtpVerification.cshtml", mailBody);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Send mail reset password to {ToEmail} FAILED", mailBody.ToEmail);
                throw;
            }
        }

        public async Task SendEmailConfirmDeleteAccount(OtpVerificationMailBody mailBody)
        {
            try
            {
                await SendMail(actionName: "SendEmailConfirmDeleteAccount", templatePath: "Views/Templates/OtpVerification.cshtml", mailBody);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Send mail confirm delete account to {ToEmail} FAILED", mailBody.ToEmail);
            }
        }

        private async Task SendMail(string actionName, string templatePath, MailBodyBase mailBody)
        {
            try
            {
                _logger.LogInformation("{ActionName}: Send email to {Email}", actionName, mailBody.ToEmail);
                using var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                emailMessage.To.Add(new MailboxAddress(mailBody.ToUserName, mailBody.ToEmail));
                emailMessage.Subject = mailBody.Subject;

                string content = await razorViewService.RenderViewToStringAsync(templatePath, mailBody);
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = content
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, useSsl: false); // nếu dùng ssl thì port là 465
                await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                _logger.LogInformation("{ActionName}: Send email to {Email} successfully", actionName, mailBody.ToEmail);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ActionName}: Send email to {Email} FAILED", actionName, mailBody.ToEmail);
                throw;
            }
        }
    }
}