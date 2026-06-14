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

        public Sale? GetById(int id)
        {
            using var db = new StoreDbContext();
            return db.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefault(s => s.Id == id);
        }

        public decimal GetTotalRevenue()
        {
            using var db = new StoreDbContext();
            return db.Sales.Sum(s => s.TotalAmount);
        }
    }
}
