using System.ComponentModel.DataAnnotations;

namespace ContactListApp.Models
{
    public class ContactSubcategory
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }


        // Parent category
        public int CategoryId { get; set; }
        public ContactCategory Category { get; set; }
    }
}
