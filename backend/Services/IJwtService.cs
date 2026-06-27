using ContactListApp.Models;

namespace ContactListApp.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
