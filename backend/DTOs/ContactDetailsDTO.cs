using System.ComponentModel.DataAnnotations;

namespace ContactListApp.DTOs
{
    public class ContactDetailsDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public string? CustomSubcategory { get; set; }
        public DateOnly BirthDate { get; set; }
        public int UserId { get; set; }
    }
}
