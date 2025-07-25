using AI_ticket_system.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_ticket_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly UserSignupEventHandler _handler;

        public EventsController(UserSignupEventHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("user-signup")]
        public async Task<IActionResult> OnUserSignup([FromBody] string email)
        {
            var result = await _handler.HandleUserSignupAsync(email);
            return result ? Ok("Welcome email sent") : StatusCode(500, "Failed to process signup");
        }
    }
}
