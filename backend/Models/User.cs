namespace ContactListApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // contacts belonging to the user
        public List<ContactItem> contacts = new();
    }
}
