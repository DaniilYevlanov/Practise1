using System.Collections.Generic;
using System.Linq;
using MagazinWPF.Data;
using MagazinWPF.Models;

namespace MagazinWPF.Services
{
    public class CategoryService
    {
        public List<Category> GetAll()
        {
            using var db = new StoreDbContext();
            return db.Categories
                .OrderBy(c => c.Id)
                .ToList();
        }

        public Category? GetById(int id)
        {
            using var db = new StoreDbContext();
            return db.Categories.Find(id);
        }

        public void Add(Category category)
        {
            using var db = new StoreDbContext();
            db.Categories.Add(category);
            db.SaveChanges();

            DataEvents.RaiseCategoriesChanged();
        }

        public void Update(Category category)
        {
            using var db = new StoreDbContext();
            var existing = db.Categories.Find(category.Id);
            if (existing == null)
            {
                return;
            }

            existing.Name        = category.Name;
            existing.Description = category.Description;
            existing.IsActive    = category.IsActive;

            db.SaveChanges();

            DataEvents.RaiseCategoriesChanged();
        }

        public bool HasProducts(int categoryId)
        {
            using var db = new StoreDbContext();
            return db.Products.Any(p => p.CategoryId == categoryId);
        }

        public bool Delete(int id)
        {
            using var db = new StoreDbContext();

            if (db.Products.Any(p => p.CategoryId == id))
            {
                return false;
            }

            var existing = db.Categories.Find(id);
            if (existing == null)
            {
                return true;
            }

            db.Categories.Remove(existing);
            db.SaveChanges();

            DataEvents.RaiseCategoriesChanged();
            return true;
        }
    }
}
