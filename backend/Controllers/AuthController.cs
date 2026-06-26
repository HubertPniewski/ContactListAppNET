using ContactListApp.DTOs;
using ContactListApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ContactListApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ContactsContext _context;

        public AuthController(ContactsContext context)
        {
            _context = context;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<ActionResult> Register(AuthDTO dto)
        {
            // ensure there's no user with given email
            var userExists = await _context.Users.AnyAsync(x => x.Email == dto.Email);
            if (userExists) return BadRequest("User with this email address already exists.");
            
            // a new user creation
            var newUser = new User 
            { 
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) // password hashed with BCrypt
            };

            // save the new user to the db
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // return a success response
            return Ok(new { message = "Registration successful!" });
        }

        // Login
        [HttpPost("login")]
        public async Task<ActionResult> Login(AuthDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            // ensure the user with given email exist
            if (user == null) return Unauthorized("Invalid email or password.");

            // password verification
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordCorrect) return Unauthorized("Invalid email or password.");

            // TODO: return JWT token
            return Ok(new { message = "Login successfull." });
        }
    }
}
