using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MagazinWPF.Data;
using MagazinWPF.Models;

namespace MagazinWPF.Services
{
    public class ProductService
    {
        public List<Product> GetAll()
        {
            using var db = new StoreDbContext();
            return db.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public List<Product> GetByCategory(int categoryId)
        {
            using var db = new StoreDbContext();
            return db.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public Product? GetById(int id)
        {
            using var db = new StoreDbContext();
            return db.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            using var db = new StoreDbContext();
            db.Products.Add(product);
            db.SaveChanges();

            DataEvents.RaiseProductsChanged();
        }

        public void Update(Product product)
        {
            using var db = new StoreDbContext();
            var existing = db.Products.Find(product.Id);
            if (existing == null)
            {
                return;
            }

            existing.Name        = product.Name;
            existing.Price       = product.Price;
            existing.Stock       = product.Stock;
            existing.CategoryId  = product.CategoryId;
            existing.Barcode     = product.Barcode;
            existing.ImagePath   = product.ImagePath;
            existing.IsAvailable = product.IsAvailable;
            existing.IsTop       = product.IsTop;
            existing.IsNew       = product.IsNew;

            db.SaveChanges();

            DataEvents.RaiseProductsChanged();
        }

        public bool Delete(int id)
        {
            using var db = new StoreDbContext();
            var existing = db.Products.Find(id);
            if (existing == null)
            {
                return true;
            }

            bool usedInSales = db.SaleItems.Any(si => si.ProductId == id);
            bool usedInCarts = db.CartItems.Any(ci => ci.ProductId == id);

            if (usedInSales || usedInCarts)
            {
                return false;
            }

            db.Products.Remove(existing);
            db.SaveChanges();

            DataEvents.RaiseProductsChanged();
            return true;
        }
    }
}
