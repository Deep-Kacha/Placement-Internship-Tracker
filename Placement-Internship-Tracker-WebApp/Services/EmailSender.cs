using System.Net;
using System.Net.Mail;

namespace PlacementTracker.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var host = _config["SmtpSettings:Host"];
                if (string.IsNullOrEmpty(host))
                {
                    _logger.LogWarning("SMTP Configuration is missing. Skipping email send.");
                    return;
                }

                int port = int.Parse(_config["SmtpSettings:Port"] ?? "587");
                string user = _config["SmtpSettings:Username"] ?? "";
                string pass = _config["SmtpSettings:Password"] ?? "";
                bool ssl = bool.Parse(_config["SmtpSettings:EnableSsl"] ?? "true");
                string senderEmail = _config["SmtpSettings:SenderEmail"] ?? user;
                string senderName = _config["SmtpSettings:SenderName"] ?? "Trackeoo";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                using var client = new SmtpClient(host, port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = ssl
                };

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Password reset email successfully sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {email}: {ex.Message}");
                throw;
            }
        }
    }
}
