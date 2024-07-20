using Lidas.UserApi.Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Lidas.UserApi.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public bool SendEmail(string userName, string userEmail, string subject, string content)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.Name, _settings.Email));
            message.To.Add(new MailboxAddress(userName, userEmail));
            message.Subject = subject;

            try
            {
                // Body
                message.Body = new TextPart("html")
                {
                    Text = content,
                };

                using (var client = new SmtpClient())
                {
                    // Use STARTTLS
                    client.Connect(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);

                    client.Authenticate(_settings.Email, _settings.Password);

                    client.Send(message);

                    client.Disconnect(true);
                }
              
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}