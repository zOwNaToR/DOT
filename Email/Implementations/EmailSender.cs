using Email.Abstractions;
using System.Net.Mail;
using System.Net;

namespace Email
{
    public class EmailSender : IEmailSender
    {
        private readonly string _defaultFrom;
        private readonly string _defaultPassword;
        private readonly string _host;
        private readonly int _port;

        public EmailSender(string defaultFrom, string defaultPassword, string host, int port)
        {
            _defaultFrom = defaultFrom;
            _defaultPassword = defaultPassword;
            _host = host;
            _port = port;
        }

        public async Task SendAsync(string from, string to, string subject, string html)
        {
            var fromAddress = new MailAddress(string.IsNullOrEmpty(from) ? _defaultFrom : from, ".NET Auth");
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
            var fromAddress = new MailAddress(string.IsNullOrEmpty(from) ? _defaultFrom : from, ".NET Auth");

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
                using var smtp = new SmtpClient(_host, _port)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(email.From?.Address, _defaultPassword),
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