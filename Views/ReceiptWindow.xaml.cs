using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    public partial class ReceiptWindow : Window
    {
        private readonly Sale _sale;

        public ReceiptWindow(Sale sale)
        {
            InitializeComponent();
            _sale = sale;
            PopulateReceipt();
        }

        private void PopulateReceipt()
        {
            ReceiptIdTextBlock.Text = $"#{_sale.Id:D6}";
            DateTextBlock.Text      = _sale.SaleDate.ToString("dd.MM.yyyy HH:mm:ss");
            CashierTextBlock.Text   = _sale.CashierName ?? "—";

            ItemsControl.ItemsSource = _sale.Items;

            TotalTextBlock.Text  = $"{_sale.TotalAmount:N2} грн";
            PaidTextBlock.Text   = _sale.AmountPaid > 0
                ? $"{_sale.AmountPaid:N2} грн"
                : $"{_sale.TotalAmount:N2} грн";
            ChangeTextBlock.Text = _sale.AmountPaid > 0
                ? $"{_sale.Change:N2} грн"
                : "0,00 грн";
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
                return;

            // Будуємо FlowDocument для друку
            var doc = new FlowDocument
            {
                PagePadding   = new Thickness(40),
                ColumnWidth   = double.MaxValue,
                FontFamily    = new FontFamily("Courier New"),
                FontSize      = 12
            };

            void AddLine(string left, string right = "")
            {
                var p = new Paragraph { Margin = new Thickness(0, 1, 0, 1) };
                if (string.IsNullOrEmpty(right))
                {
                    p.Inlines.Add(new Run(left));
                }
                else
                {
                    // вирівнювання правої частини
                    int total = 42;
                    int spaces = total - left.Length - right.Length;
                    if (spaces < 1) spaces = 1;
                    p.Inlines.Add(new Run(left + new string(' ', spaces) + right));
                }
                doc.Blocks.Add(p);
            }

            void AddSeparator() => AddLine(new string('-', 42));

            void AddCenter(string text)
            {
                var p = new Paragraph(new Run(text))
                {
                    TextAlignment = TextAlignment.Center,
                    Margin        = new Thickness(0, 2, 0, 2)
                };
                doc.Blocks.Add(p);
            }

            AddCenter("*** МАГАЗИН ***");
            AddCenter("ФІСКАЛЬНИЙ ЧЕК");
            AddSeparator();
            AddLine($"Чек №{_sale.Id:D6}");
            AddLine($"Дата: {_sale.SaleDate:dd.MM.yyyy HH:mm:ss}");
            AddLine($"Касир: {_sale.CashierName ?? "—"}");
            AddSeparator();
            AddLine("Товар", "Сума");
            AddSeparator();

            foreach (var item in _sale.Items)
            {
                string name = item.Product?.Name ?? $"Товар #{item.ProductId}";
                if (name.Length > 26) name = name[..23] + "...";
                AddLine(name, $"{item.Subtotal:N2}");
                AddLine($"  {item.Quantity} x {item.UnitPrice:N2}");
            }

            AddSeparator();
            AddLine("РАЗОМ:", $"{_sale.TotalAmount:N2} грн");

            decimal paid   = _sale.AmountPaid > 0 ? _sale.AmountPaid : _sale.TotalAmount;
            decimal change = _sale.AmountPaid > 0 ? _sale.Change : 0;
            AddLine("Сплачено:", $"{paid:N2} грн");
            AddLine("Решта:",    $"{change:N2} грн");
            AddSeparator();
            AddCenter("Дякуємо за покупку!");

            var idps = new DocumentPaginator[] { };
            ((IDocumentPaginatorSource)doc).DocumentPaginator.PageSize =
                new System.Windows.Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            printDialog.PrintDocument(
                ((IDocumentPaginatorSource)doc).DocumentPaginator,
                $"Чек №{_sale.Id:D6}");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
