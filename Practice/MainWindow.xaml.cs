using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Practice
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<CartItem> CartItems { get; set; }
        public ObservableCollection<SaleRecord> SalesHistory { get; set; }

        private List<Product> _allProducts;
        private int _checkCounter = 1043;

        private decimal _subtotal;
        public decimal Subtotal
        {
            get => _subtotal;
            set { _subtotal = value; OnPropertyChanged(nameof(Subtotal)); OnPropertyChanged(nameof(Change)); }
        }

        // ---------- Сума "Отримано від клієнта" ----------

        private decimal _received;
        public decimal Received
        {
            get => _received;
            private set
            {
                _received = value;
                OnPropertyChanged(nameof(Change));
            }
        }

        private string _receivedInput = "";
        public string ReceivedInput
        {
            get => _receivedInput;
            set
            {
                _receivedInput = value;
                OnPropertyChanged(nameof(ReceivedInput));

                var normalized = value?.Replace(',', '.') ?? "";
                Received = decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
                    ? parsed
                    : 0;
            }
        }

        public decimal Change => Received - Subtotal;

        public MainWindow()
        {
            // 1. Спочатку готуємо дані
            _allProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Хліб «Дарницький»",        Icon = "🥖", Price = 32.50m,  Stock = 24, Category = "Хлібобулочні" },
                new Product { Id = 2, Name = "Молоко «Селянське» 1л",    Icon = "🥛", Price = 44.90m,  Stock = 12, Category = "Молочні продукти" },
                new Product { Id = 3, Name = "Сік «Садочок» 1л",         Icon = "🧃", Price = 58.00m,  Stock = 8,  Category = "Напої" },
                new Product { Id = 4, Name = "Шоколад «Корона»",         Icon = "🍫", Price = 29.90m,  Stock = 3,  Category = "Кондитерські" },
                new Product { Id = 5, Name = "Гель для душу 250мл",      Icon = "🧴", Price = 89.00m,  Stock = 17, Category = "Побутова хімія" },
                new Product { Id = 6, Name = "Кава розчинна 100г",       Icon = "☕", Price = 124.00m, Stock = 9,  Category = "Напої" },
            };

            Products = new ObservableCollection<Product>(_allProducts);
            CartItems = new ObservableCollection<CartItem>();
            SalesHistory = new ObservableCollection<SaleRecord>();

            // 2. Тільки після цього будуємо XAML
            InitializeComponent();

            // 3. І вже потім прив'язуємо DataContext
            DataContext = this;
        }

        // ---------- Фільтр категорій + пошук ----------

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (Products == null || _allProducts == null)
                return;

            var selected = CategoryList?.SelectedItem as ListBoxItem;
            var category = selected?.Content?.ToString();

            IEnumerable<Product> result = _allProducts;

            if (!string.IsNullOrEmpty(category) && category != "Усі товари")
                result = result.Where(p => p.Category == category);

            var query = SearchBox?.Text?.Trim();
            if (!string.IsNullOrEmpty(query))
                result = result.Where(p => p.Name.ToLower().Contains(query.ToLower()));

            Products.Clear();
            foreach (var p in result)
                Products.Add(p);
        }

        // ---------- Кошик ----------

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var product = (Product)((Button)sender).Tag;

            if (product.Stock <= 0)
            {
                MessageBox.Show($"«{product.Name}» немає на залишку.", "Немає товару",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = CartItems.FirstOrDefault(c => c.Product.Id == product.Id);
            if (existing != null)
            {
                if (existing.Quantity >= product.Stock)
                {
                    MessageBox.Show("Досягнуто максимальну кількість на залишку.", "Залишок",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                existing.Quantity++;
            }
            else
            {
                var item = new CartItem { Product = product, Quantity = 1 };
                item.PropertyChanged += (s, args) => RecalculateTotals();
                CartItems.Add(item);
            }

            RecalculateTotals();
        }

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((Button)sender).Tag;
            if (item.Quantity < item.Product.Stock)
                item.Quantity++;
            else
                MessageBox.Show("Досягнуто максимальну кількість на залишку.", "Залишок",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((Button)sender).Tag;
            if (item.Quantity > 1)
                item.Quantity--;
            else
                RemoveFromCart(item);
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (CartItem)((Button)sender).Tag;
            RemoveFromCart(item);
        }

        private void RemoveFromCart(CartItem item)
        {
            CartItems.Remove(item);
            RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            Subtotal = CartItems.Sum(i => i.LineTotal);
        }

        // ---------- Оформлення продажу ----------

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Кошик порожній.", "Продаж", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Received < Subtotal)
            {
                MessageBox.Show("Сума, отримана від клієнта, менша за суму до сплати.", "Недостатньо коштів",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var item in CartItems)
                item.Product.Stock -= item.Quantity;

            SalesHistory.Insert(0, new SaleRecord
            {
                Number = $"№{_checkCounter++}",
                DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                ItemsCount = CartItems.Sum(i => i.Quantity),
                Total = $"{Subtotal:0.00} ₴",
                Paid = $"{Received:0.00} ₴",
                Change = $"{Change:0.00} ₴",
                Cashier = "Олена К."
            });

            CartItems.Clear();
            ReceivedInput = "";
            RecalculateTotals();
            ApplyFilters();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CartItems.Clear();
            ReceivedInput = "";
            RecalculateTotals();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}