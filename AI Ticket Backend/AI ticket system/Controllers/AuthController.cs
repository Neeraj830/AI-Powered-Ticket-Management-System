using AI_ticket_system.Data;
using AI_ticket_system.DTOs;
using AI_ticket_system.Models;
using AI_ticket_system.Services;
using AI_ticket_system.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;

namespace AI_ticket_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IEventService _eventService;

        public AuthController(ApplicationDbContext context, TokenService tokenService, IEventService eventService)
        {
            _context = context;
            _tokenService = tokenService;
            _eventService = eventService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "Email already exists" });

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                Password = passwordHash,
                Role = dto.Role,
                Skills = dto.Skills ?? new List<string>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _eventService.SendUserSignupEventAsync(user.Email);

            var token = _tokenService.GenerateToken(user);
            return Ok(new { user, token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { error = "User not found" });

            var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!isValid)
                return Unauthorized(new { error = "Invalid credentials" });

            var token = _tokenService.GenerateToken(user);
            return Ok(new { user, token });
        }


        [Authorize(Roles = "admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(new { error = "User not found" });

            user.Role = dto.Role;
            user.Skills = dto.Skills ?? user.Skills;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Role,
                    u.Skills
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
