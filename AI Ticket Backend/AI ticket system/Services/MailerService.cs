using System.Net;
using System.Net.Mail;
using AI_ticket_system.Services.IServices;

namespace AI_ticket_system.Services
{
    public class MailerService : IMailerService
    {
        private readonly IConfiguration _configuration;

        public MailerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(string to, string subject, string body)
        {
            var smtpHost = _configuration["Mail:Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Mail:Smtp:Port"]);
            var fromEmail = _configuration["Mail:Smtp:User"];
            var password = _configuration["Mail:Smtp:Pass"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);

        }
    }

}
