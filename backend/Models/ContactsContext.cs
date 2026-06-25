using Microsoft.EntityFrameworkCore;

namespace ContactListApp.Models
{
    public class ContactsContext : DbContext
    {
        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options) {}

        public DbSet<ContactItem> ContactItems { get; set; } = null!;
        public DbSet<ContactCategory> ContactCategories { get; set; } = null!;
        public DbSet<ContactSubcategory> ContactSubcategories { get; set; } = null!;


        // Called while creating model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enforcing email uniqueness
            modelBuilder.Entity<ContactItem>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}
