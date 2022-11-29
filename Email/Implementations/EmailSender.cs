using Email.Abstractions;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using Email.DTOs;

namespace Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _emailConfig;

        public EmailSender(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public async Task SendAsync(string from, string to, string subject, string html)
        {
            var actualFrom = string.IsNullOrEmpty(from) ? _emailConfig.DefaultFrom : from;
            var fromAddress = new MailAddress(actualFrom, ".NET Auth");
            var toAddress = new MailAddress(to, to);

            using var email = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = html,
                IsBodyHtml = true,
            };

            await SendAsync(email);
        }
        public async Task SendAsync(string from, IEnumerable<string> to, string subject, string html)
        {
            var actualFrom = string.IsNullOrEmpty(from) ? _emailConfig.DefaultFrom : from;
            var fromAddress = new MailAddress(actualFrom, ".NET Auth");

            using var email = new MailMessage()
            {
                From = fromAddress,
                Subject = subject,
                Body = html,
                IsBodyHtml = true,
            };

            foreach (var recipient in to)
            {
                email.To.Add(recipient);
            }

            await SendAsync(email);
        }

        private async Task SendAsync(MailMessage email)
        {
            try
            {
                var credentials = !string.IsNullOrEmpty(_emailConfig.DefaultPassword)
                    ? new NetworkCredential(email.From?.Address, _emailConfig.DefaultPassword)
                    : null;

                using var smtp = new SmtpClient(_emailConfig.Host, _emailConfig.Port)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = credentials,
                    Timeout = 20000,
                };

                await smtp.SendMailAsync(email);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}