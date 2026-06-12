using Microsoft.EntityFrameworkCore;
using MagazinWPF.Models;

namespace MagazinWPF.Data
{
    /// <summary>
    /// Контекст бази даних магазину (EF Core, Code First, SQLite).
    /// Схема узгоджена командою: Category, Product, Cart, CartItem, Sale, SaleItem.
    /// </summary>
    public class StoreDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Cart> Carts => Set<Cart>();

        public DbSet<CartItem> CartItems => Set<CartItem>();

        public DbSet<Sale> Sales => Set<Sale>();

        public DbSet<SaleItem> SaleItems => Set<SaleItem>();

        // TODO: команда, яка реалізує авторизацію (User/Admin/Customer),
        // може додати сюди DbSet<User> з налаштуванням TPH-наслідування
        // (HasDiscriminator) у методі OnModelCreating.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Store.db");
        }
    }
}
