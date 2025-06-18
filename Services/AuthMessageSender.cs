using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace PurchaseOrderManagementSystem.Services
{
    public class AuthMessageSender : IEmailSender
    {
        private readonly ILogger<AuthMessageSender> _logger;
        private readonly IConfiguration _configuration;

        public AuthMessageSender(ILogger<AuthMessageSender> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");
            var userName = smtpSettings["UserName"];
            var password = smtpSettings["Password"];

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = enableSsl;
                client.Credentials = new System.Net.NetworkCredential(userName, password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(userName ?? "noreply@example.com"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                try
                {
                    _logger.LogInformation($"Attempting to send email to {email} with subject '{subject}'.");
                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent to {email} successfully.");
                }
                catch (SmtpException ex)
                {
                    _logger.LogError(ex, $"Failed to send email to {email}. SMTP Error: {ex.StatusCode} - {ex.Message}");
                    throw; // Re-throw the exception to indicate failure
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An unexpected error occurred while sending email to {email}. Error: {ex.Message}");
                    throw; // Re-throw the exception to indicate failure
                }
            }
        }
    }
}

