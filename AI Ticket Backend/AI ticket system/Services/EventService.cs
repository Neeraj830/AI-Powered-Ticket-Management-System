using AI_ticket_system.Models;
using AI_ticket_system.Services.IServices;

namespace AI_ticket_system.Services
{
    public class EventService : IEventService
    {
        public async Task SendTicketCreatedEventAsync(Ticket ticket)
        {
            Console.WriteLine($"Event sent: ticket/created, Ticket ID: {ticket.Id}");
            await Task.CompletedTask;
        }

        public async Task SendUserSignupEventAsync(string email)
        {
            Console.WriteLine($"Event: user/signup - {email}");
            await Task.CompletedTask;
        }

    }
}
