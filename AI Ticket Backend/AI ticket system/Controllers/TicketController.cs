using System.Security.Claims;
using AI_ticket_system.Data;
using AI_ticket_system.DTOs;
using AI_ticket_system.Models;
using AI_ticket_system.Services;
using AI_ticket_system.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AI_ticket_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;
        private readonly ITicketCreatedEventHandler _ticketCreatedEventHandler;

        public TicketController(ApplicationDbContext context, IEventService eventService, ITicketCreatedEventHandler ticketCreatedEventHandler)
        {
            _context = context;
            _eventService = eventService;
            _ticketCreatedEventHandler = ticketCreatedEventHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketDto ticketDto)
        {
            if (string.IsNullOrWhiteSpace(ticketDto.Title) || string.IsNullOrWhiteSpace(ticketDto.Description))
            {
                return BadRequest(new { message = "Title and description are required" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ticket = new Ticket
            {
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                CreatedById = Guid.Parse(userId)
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();


            await _eventService.SendTicketCreatedEventAsync(ticket);

            return StatusCode(201, new { message = "Ticket created and processing started", ticket });
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "user")
            {
                var tickets = await _context.Tickets
                    .Include(t => t.AssignedTo) 
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return Ok(tickets);
            }
            else
            {
                var tickets = await _context.Tickets
                    .Where(t => t.CreatedById == Guid.Parse(userId))
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new
                    {
                        t.Title,
                        t.Description,
                        t.Status,
                        t.CreatedAt
                    })
                    .ToListAsync();

                return Ok(tickets);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            Ticket? ticket;

            if (userRole != "user")
            {
                ticket = await _context.Tickets
                    .Include(t => t.AssignedTo)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
            else
            {
                ticket = await _context.Tickets
                    .Where(t => t.CreatedById == Guid.Parse(userId) && t.Id == id)
                    .Select(t => new Ticket
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        Status = t.Status,
                        CreatedAt = t.CreatedAt
                    })
                    .FirstOrDefaultAsync();
            }

            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            var tbool = _ticketCreatedEventHandler.HandleAsync(ticket.Id);
            Console.WriteLine("Ticket is assigned to someone : ", tbool);
            return Ok(new { ticket });
        }
    }
}
