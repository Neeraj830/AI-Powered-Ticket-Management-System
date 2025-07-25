using System.ComponentModel.DataAnnotations;

namespace AI_ticket_system.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required]
        public string Role { get; set; } = "user"; 

        public List<string> Skills { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Ticket>? CreatedTickets { get; set; }
        public ICollection<Ticket>? AssignedTickets { get; set; }

    }

}
