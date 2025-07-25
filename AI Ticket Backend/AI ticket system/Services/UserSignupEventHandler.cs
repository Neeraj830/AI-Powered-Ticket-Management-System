using AI_ticket_system.Data;
using AI_ticket_system.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace AI_ticket_system.Services
{
    public class UserSignupEventHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly IMailerService _mailer;

        public UserSignupEventHandler(ApplicationDbContext context, IMailerService mailer)
        {
            _context = context;
            _mailer = mailer;
        }

        public async Task<bool> HandleUserSignupAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    throw new InvalidOperationException("User no longer exists in our database");
                }

                string subject = "Welcome to the app";
                string message = @"Hi,

Thanks for signing up. We're glad to have you onboard!";

                await _mailer.SendMailAsync(user.Email, subject, message);

                return true;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Non-retriable error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error running step: {ex.Message}");
                return false;
            }
        }
    }
}
