using System.ComponentModel.DataAnnotations;

namespace AI_ticket_system.DTOs
{
    public class TicketDto
    {
        [Required]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }
    }
}
