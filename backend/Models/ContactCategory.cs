using System.ComponentModel.DataAnnotations;

namespace ContactListApp.Models
{
    public class ContactCategory
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        // Subcategories 
        public ICollection<ContactSubcategory> Subcategories { get; set; } = new List<ContactSubcategory>();
    }
}
