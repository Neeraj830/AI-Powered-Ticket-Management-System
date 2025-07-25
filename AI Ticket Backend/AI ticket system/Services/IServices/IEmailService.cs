namespace AI_ticket_system.Services.IServices
{
    public interface IEmailService
    {
        Task SendMailAsync(string to, string subject, string text);
    }

}
