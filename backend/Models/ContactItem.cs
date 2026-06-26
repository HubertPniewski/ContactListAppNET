using System.ComponentModel.DataAnnotations;

namespace ContactListApp.Models
{
    public class ContactItem
    {
        public long Id { get; set; }

        // Personal data
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateOnly BirthDate { get; set; }

        // Contact data
        [MaxLength(256)]
        public string Email { get; set; }
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        // Password hash
        public string PasswordHash { get; set; }

        // Contact categories
        public int CategoryId { get; set; }
        public ContactCategory Category { get; set; }
        public int? SubcategoryId {  get; set; }
        public ContactSubcategory? Subcategory { get; set; }
        [MaxLength(100)]
        public string? CustomSubcategory { get; set; }

        // User owning the contact
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
