using AI_ticket_system.Data;
using AI_ticket_system.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace AI_ticket_system.Services
{
    public interface ITicketCreatedEventHandler
    {
        Task<bool> HandleAsync(Guid ticketId);
    }

    public class TicketCreatedEventHandler : ITicketCreatedEventHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly IMailerService _mailer;
        private readonly IAiService _aiService;

        public TicketCreatedEventHandler(ApplicationDbContext context, IMailerService mailer, IAiService aiService)
        {
            _context = context;
            _mailer = mailer;
            _aiService = aiService;
        }

        public async Task<bool> HandleAsync(Guid ticketId)
        {
            try
            {
                var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticket == null)
                {
                    throw new InvalidOperationException("Ticket not found");
                }


                var aiResult = await _aiService.AnalyzeTicketAsync(ticket);
                if (aiResult != null)
                {
                    ticket.Priority = new[] { "low", "medium", "high" }
                        .Contains(aiResult.Priority?.ToLower()) ? aiResult.Priority : "medium";

                    ticket.HelpfulNotes = aiResult.HelpfulNotes;
                    ticket.Status = "IN_PROGRESS";
                    ticket.RelatedSkills = aiResult.RelatedSkills ?? new List<string>();
                    await _context.SaveChangesAsync();
                }

                var skillsRegex = aiResult?.RelatedSkills ?? new List<string>();
                var matchedModerator = await _context.Users
                    .Where(u => u.Role == "moderator" &&
                                u.Skills.Any(skill =>
                                    skillsRegex.Any(rs => EF.Functions.Like(skill, $"%{rs}%"))))
                    .FirstOrDefaultAsync();

                if (matchedModerator == null)
                {
                    matchedModerator = await _context.Users
                        .Where(u => u.Role == "admin")
                        .FirstOrDefaultAsync();
                }

                if (matchedModerator != null)
                {
                    ticket.AssignedTo = matchedModerator;
                    await _context.SaveChangesAsync();

                    await _mailer.SendMailAsync(
                        matchedModerator.Email,
                        "Ticket Assigned",
                        $"A new ticket is assigned to you: {ticket.Title}"
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing ticket/created event: {ex.Message}");
                return false;
            }
        }
    }
}
