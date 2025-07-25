using System.Net;
using System.Net.Mail;
using AI_ticket_system.Services.IServices;

namespace AI_ticket_system.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMailAsync(string to, string subject, string text)
        {
            try
            {
                var smtpHost = _config["Mail:Smtp:Host"];
                var smtpPort = int.Parse(_config["Mail:Smtp:Port"]);
                var smtpUser = _config["Mail:Smtp:User"];
                var smtpPass = _config["Mail:Smtp:Pass"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = false // Set to true if needed
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("no-reply@inngest.local", "Inngest TMS"),
                    Subject = subject,
                    Body = text,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);

                Console.WriteLine("✅ Email sent successfully to: " + to);

                //await _emailService.SendMailAsync("user@example.com", "Welcome", "Hello from Inngest TMS!");

            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Mail error: " + ex.Message);
                throw;
            }
        }

    }
}
