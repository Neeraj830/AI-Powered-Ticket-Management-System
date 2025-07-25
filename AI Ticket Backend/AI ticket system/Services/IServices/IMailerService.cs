namespace AI_ticket_system.Services.IServices
{
    public interface IMailerService
    {
        Task SendMailAsync(string to, string subject, string body);
    }

}
