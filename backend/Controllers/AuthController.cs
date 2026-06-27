using ContactListApp.DTOs;
using ContactListApp.Models;
using ContactListApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ContactListApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ContactsContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(ContactsContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
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

            // ensure the user with given email exist and the password is valid
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) 
                return Unauthorized("Invalid email or password.");

            // generating JWT token
            var token = _jwtService.GenerateToken(user);

            // return JWT token
            return Ok(new { token, email = user.Email });
        }
    }
}
