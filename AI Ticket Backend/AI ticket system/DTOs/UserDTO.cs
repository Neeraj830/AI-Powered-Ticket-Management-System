namespace AI_ticket_system.DTOs
{
    public class SignupDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<string>? Skills { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public List<string>? Skills { get; set; }
    }

}
