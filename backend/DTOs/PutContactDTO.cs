using System.ComponentModel.DataAnnotations;

namespace ContactListApp.DTOs
{
    public class PutContactDTO
    {
        [Required] public long Id { get; set; }
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Phone { get; set; } = string.Empty;
        [Required] public int CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public string? CustomSubcategory { get; set; }
        [Required] public DateOnly BirthDate { get; set; }
    }
}
