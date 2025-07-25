using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_ticket_system.Models
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public string Status { get; set; } = "TODO";

        public Guid CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public User? CreatedBy { get; set; }

        public Guid? AssignedToId { get; set; }
        [ForeignKey("AssignedToId")]
        public User? AssignedTo { get; set; }

        public string? Priority { get; set; }

        public DateTime? Deadline { get; set; }

        public string? HelpfulNotes { get; set; }

        public List<string> RelatedSkills { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }

}
