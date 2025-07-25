using AI_ticket_system.Models;

namespace AI_ticket_system.Services.IServices
{
    public interface IAiService
    {
        Task<AiResponse?> AnalyzeTicketAsync(Ticket ticket);
    }

    public class AiResponse
    {
        public string Summary { get; set; }
        public string Priority { get; set; }
        public string HelpfulNotes { get; set; }
        public List<string> RelatedSkills { get; set; }
    }


}
