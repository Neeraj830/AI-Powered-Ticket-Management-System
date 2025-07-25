using AI_ticket_system.Models;

namespace AI_ticket_system.Services.IServices
{
    public interface IEventService
    {
        Task SendTicketCreatedEventAsync(Ticket ticket);
        Task SendUserSignupEventAsync(string email);
    }

}
