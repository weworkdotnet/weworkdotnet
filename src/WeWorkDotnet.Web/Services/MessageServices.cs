using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using WeWorkDotnet.Web.Models.ConfigurationModels;

namespace WeWorkDotnet.Web.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly SendGridConfig _sendGridConfig;

        public AuthMessageSender(IOptions<SendGridConfig> sendGridConfig)
        {
            _sendGridConfig = sendGridConfig.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await SendGridAsync(email, subject, message);
        }

        private async Task SendGridAsync(string email, string subject, string message)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_sendGridConfig.FromEmail, _sendGridConfig.FromName),
                Subject = subject,
                HtmlContent = message,
                PlainTextContent = message,
                TrackingSettings = new TrackingSettings
                {
                    ClickTracking = new ClickTracking
                    {
                        Enable = true,
                        EnableText = true
                    }
                }

            };

            msg.AddTo(new EmailAddress(email));

            var client = new SendGridClient(_sendGridConfig.ApiKey);
            var response = await client.SendEmailAsync(msg);

        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
