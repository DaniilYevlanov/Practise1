using Microsoft.EntityFrameworkCore;
using MagazinWPF.Models;

namespace MagazinWPF.Data
{
    public class StoreDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleItem> SaleItems => Set<SaleItem>();
        public DbSet<UserAccount> Users => Set<UserAccount>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Store.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed початкових користувачів
            modelBuilder.Entity<UserAccount>().HasData(
                new UserAccount
                {
                    Id = 1,
                    Login = "admin",
                    PasswordHash = UserAccount.HashPassword("admin123"),
                    FullName = "Головний адміністратор",
                    Role = "Admin"
                },
                new UserAccount
                {
                    Id = 2,
                    Login = "user",
                    PasswordHash = UserAccount.HashPassword("user123"),
                    FullName = "Тестовий покупець",
                    Role = "Customer"
                }
            );
        }
    }
}
