using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MagazinWPF.Data;
using MagazinWPF.Models;

namespace MagazinWPF.Services
{
    public class SaleService
    {
        public List<Sale> GetAll()
        {
            using var db = new StoreDbContext();
            return db.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToList();
        }

        public List<Sale> GetByCustomer(string login)
        {
            using var db = new StoreDbContext();
            return db.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Where(s => s.CashierName == login)
                .OrderByDescending(s => s.SaleDate)
                .ToList();
        }

        public Sale? GetById(int id)
        {
            using var db = new StoreDbContext();
            return db.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefault(s => s.Id == id);
        }

        public void Add(Sale sale)
        {
            using var db = new StoreDbContext();
            db.Sales.Add(sale);
            db.SaveChanges();
        }

        public decimal GetTotalRevenue()
        {
            using var db = new StoreDbContext();
            return db.Sales.Sum(s => s.TotalAmount);
        }
    }
}