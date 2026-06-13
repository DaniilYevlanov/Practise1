using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    /// <summary>
    /// Вікно покупця: каталог товарів, категорії та кошик.
    /// Демо-версія: дані зберігаються в пам'яті (без БД).
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        // Усі товари (демо-дані, не фільтровані)
        private List<Product> _allProducts = new();

        // Усі категорії (демо-дані)
        private List<Category> _allCategories = new();

        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<CartItem> CartItems { get; } = new();

        public MainWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            UserNameTextBlock.Text = $"{currentUser.FullName} ({currentUser.Role})";

            ProductsItemsControl.ItemsSource = Products;
            CartItemsControl.ItemsSource = CartItems;

            LoadDemoCategories();
            LoadDemoProducts();

            ApplyFilter();
            RefreshCart();
        }

        // ── Демо-дані ─────────────────────────────────────────────────

        private void LoadDemoCategories()
        {
            _allCategories.Add(new Category { Id = 1, Name = "Смартфони", IsActive = true });
            _allCategories.Add(new Category { Id = 2, Name = "Ноутбуки", IsActive = true });
            _allCategories.Add(new Category { Id = 3, Name = "Периферія", IsActive = true });

            // Перший пункт "Усі товари" вже є в XAML і має Tag = null.
            // Додаємо решту категорій у ListBox динамічно.
            foreach (var cat in _allCategories)
            {
                CategoriesListBox.Items.Add(new ListBoxItem
                {
                    Content = cat.Name,
                    Tag = cat.Id
                });
            }
        }

        private void LoadDemoProducts()
        {
            _allProducts.Add(new Product { Id = 1, Name = "Samsung A26", Price = 9899, Stock = 10, CategoryId = 1, IsAvailable = true });
            _allProducts.Add(new Product { Id = 2, Name = "Xiaomi 15", Price = 36999, Stock = 4, CategoryId = 1, IsAvailable = true });
            _allProducts.Add(new Product { Id = 3, Name = "iPhone 15", Price = 42999, Stock = 6, CategoryId = 1, IsAvailable = true });
            _allProducts.Add(new Product { Id = 4, Name = "Lenovo ThinkPad", Price = 35000, Stock = 5, CategoryId = 2, IsAvailable = true });
            _allProducts.Add(new Product { Id = 5, Name = "Asus Vivobook", Price = 28000, Stock = 7, CategoryId = 2, IsAvailable = true });
            _allProducts.Add(new Product { Id = 6, Name = "HP Pavilion", Price = 31500, Stock = 3, CategoryId = 2, IsAvailable = true });
            _allProducts.Add(new Product { Id = 7, Name = "Logitech Mouse", Price = 800, Stock = 20, CategoryId = 3, IsAvailable = true });
            _allProducts.Add(new Product { Id = 8, Name = "Proove Gaming", Price = 1099, Stock = 15, CategoryId = 3, IsAvailable = true });
            _allProducts.Add(new Product { Id = 9, Name = "Keychron K2", Price = 3200, Stock = 8, CategoryId = 3, IsAvailable = true });
        }

        // ── Фільтрація за категорією ──────────────────────────────────

        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Products.Clear();

            int? selectedCategoryId = null;

            if (CategoriesListBox.SelectedItem is ListBoxItem item && item.Tag is int catId)
                selectedCategoryId = catId;

            var filtered = selectedCategoryId.HasValue
                ? _allProducts.Where(p => p.CategoryId == selectedCategoryId.Value && p.IsAvailable)
                : _allProducts.Where(p => p.IsAvailable);

            foreach (var p in filtered)
                Products.Add(p);
        }

        // ── Кошик ─────────────────────────────────────────────────────

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Product product)
                return;

            var existing = CartItems.FirstOrDefault(i => i.ProductId == product.Id);
            int currentQty = existing?.Quantity ?? 0;

            if (currentQty >= product.Stock)
            {
                MessageBox.Show($"У наявності лише {product.Stock} шт. товару «{product.Name}».",
                    "Недостатньо товару", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1,
                    UnitPrice = product.Price
                });
            }

            RefreshCart();
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not CartItem item)
                return;

            int stock = item.Product?.Stock ?? 0;

            if (item.Quantity >= stock)
            {
                MessageBox.Show($"У наявності лише {stock} шт. товару «{item.Product?.Name}».",
                    "Недостатньо товару", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            item.Quantity++;
            RefreshCart();
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not CartItem item)
                return;

            item.Quantity--;

            if (item.Quantity <= 0)
                CartItems.Remove(item);

            RefreshCart();
        }

        private void RefreshCart()
        {
            CartItemsControl.Items.Refresh();

            bool isEmpty = CartItems.Count == 0;
            EmptyCartTextBlock.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
            CartItemsControl.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;

            decimal total = CartItems.Sum(i => i.TotalPrice);
            TotalAmountTextBlock.Text = $"{total:N0}";
        }

        // ── Оформлення замовлення ──────────────────────────────────────

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Кошик порожній.", "Магазин",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Підтвердити замовлення на суму {CartItems.Sum(i => i.TotalPrice):N0} грн?",
                "Оформлення замовлення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            // Перевіряємо залишки ще раз (про всяк випадок)
            foreach (var cartItem in CartItems)
            {
                if (cartItem.Product == null || cartItem.Quantity > cartItem.Product.Stock)
                {
                    MessageBox.Show(
                        $"Недостатньо товару «{cartItem.Product?.Name}».",
                        "Недостатньо товару", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            decimal total = CartItems.Sum(i => i.TotalPrice);

            // Зменшуємо залишки локально
            foreach (var cartItem in CartItems)
            {
                var product = _allProducts.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product != null)
                    product.Stock -= cartItem.Quantity;
            }

            CartItems.Clear();
            RefreshCart();
            ApplyFilter(); // оновити відображення залишків

            MessageBox.Show(
                $"Замовлення успішно оформлено!\nСума: {total:N0} грн.",
                "Дякуємо за покупку!",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // ── Вихід ─────────────────────────────────────────────────────

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}
