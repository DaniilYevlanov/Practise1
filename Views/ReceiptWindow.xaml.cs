using System.Globalization;
using System.Linq;
using System.Windows;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    public partial class ReceiptWindow : Window
    {
        private class ReceiptLine
        {
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Subtotal => Quantity * UnitPrice;
        }

        public ReceiptWindow(Sale sale)
        {
            InitializeComponent();

            SaleIdTextBlock.Text = $"#{sale.Id}";
            SaleDateTextBlock.Text = sale.SaleDate.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            CustomerNameTextBlock.Text = sale.CashierName ?? "—";

            var lines = sale.Items
                .Select(i => new ReceiptLine
                {
                    ProductName = i.Product?.Name ?? "Товар",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                })
                .ToList();

            ItemsListControl.ItemsSource = lines;
            TotalTextBlock.Text = sale.TotalAmount.ToString("N0", CultureInfo.InvariantCulture);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}