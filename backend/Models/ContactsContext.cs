using Microsoft.EntityFrameworkCore;

namespace ContactListApp.Models
{
    public class ContactsContext : DbContext
    {
        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }

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

            // Ensure contact categories exist
            modelBuilder.Entity<ContactCategory>().HasData(
                new ContactCategory { Id = 1, Name = "s³u¿bowy" },
                new ContactCategory { Id = 2, Name = "prywatny" },
                new ContactCategory { Id = 3, Name = "inny" }
            );

            // Ensure contact subcategories for "s³u¿bowy" category exist
            modelBuilder.Entity<ContactSubcategory>().HasData(
                new ContactSubcategory { Id = 1, Name = "szef", CategoryId = 1 },
                new ContactSubcategory { Id = 2, Name = "klient", CategoryId = 1 },
                new ContactSubcategory { Id = 3, Name = "pracownik dzia³u IT", CategoryId = 1 },
                new ContactSubcategory { Id = 4, Name = "ksiêgowy/a", CategoryId = 1 }
            );
        }
    }
}
