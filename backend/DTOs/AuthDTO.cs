using System.ComponentModel.DataAnnotations;

namespace ContactListApp.DTOs
{
    public class AuthDTO
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}
