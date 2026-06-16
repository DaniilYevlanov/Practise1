using System;
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

        public Sale Checkout(IEnumerable<CartItem> cartItems, string? cashierName)
        {
            using var db = new StoreDbContext();

            var sale = new Sale
            {
                CashierName = cashierName
            };

            decimal total = 0;

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Product == null)
                    continue;

                var product = db.Products.Find(cartItem.Product.Id);
                if (product == null)
                    continue;

                if (cartItem.Quantity > product.Stock)
                {
                    throw new InvalidOperationException(
                        $"Недостатньо товару «{product.Name}». В наявності: {product.Stock} шт.");
                }

                product.Stock -= cartItem.Quantity;

                var saleItem = new SaleItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice
                };

                sale.Items.Add(saleItem);
                total += saleItem.Subtotal;
            }

            sale.TotalAmount = total;
            sale.AmountPaid = total;
            sale.Change = 0;

            db.Sales.Add(sale);
            db.SaveChanges();

            DataEvents.RaiseProductsChanged();
            DataEvents.RaiseSalesChanged();

            return sale;
        }
    }
}