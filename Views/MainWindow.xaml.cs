using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    /// <summary>
    /// Вікно покупця: каталог товарів, категорії та кошик.
    /// Наразі реалізовано лише зовнішній вигляд та найпростішу взаємодію елементів —
    /// реальну роботу з базою даних (EF Core / SQLite) та правила (Services)
    /// додасть команда бізнес-логіки.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public ObservableCollection<Product> Products { get; } = new();

        public ObservableCollection<CartItem> CartItems { get; } = new();

        public MainWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            UserNameTextBlock.Text = $"{currentUser.FullName} ({currentUser.Role})";

            // Демонстраційні дані для зовнішнього вигляду.
            // TODO: команда замінить це на завантаження категорій і товарів
            // з бази даних через StoreDbContext / ProductService.
            LoadDemoProducts();

            ProductsItemsControl.ItemsSource = Products;
            CartItemsControl.ItemsSource = CartItems;
            RefreshCart();
        }

        private void LoadDemoProducts()
        {
            Products.Add(new Product { Id = 1, Name = "Samsung A26", Price = 9899, Stock = 10, CategoryId = 1 });
            Products.Add(new Product { Id = 2, Name = "Xiaomi 15", Price = 36999, Stock = 4, CategoryId = 1 });
            Products.Add(new Product { Id = 3, Name = "Lenovo ThinkPad", Price = 35000, Stock = 5, CategoryId = 2 });
            Products.Add(new Product { Id = 4, Name = "Asus Vivobook", Price = 28000, Stock = 7, CategoryId = 2 });
            Products.Add(new Product { Id = 5, Name = "Logitech Mouse", Price = 800, Stock = 20, CategoryId = 3 });
            Products.Add(new Product { Id = 6, Name = "Proove Gaming", Price = 1099, Stock = 15, CategoryId = 3 });
        }

        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: команда реалізує фільтрацію товарів за обраною категорією
            // (наприклад, через ProductService.GetByCategory(categoryId)).
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            // Проста локальна логіка лише для того, щоб кошик виглядав "живим".
            // TODO: команда замінить на CartService.AddProduct(...) з перевіркою
            // залишку товару (Stock) та збереженням у базу.
            if (sender is Button button && button.Tag is Product product)
            {
                var existing = CartItems.FirstOrDefault(i => i.Product?.Id == product.Id);

                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    CartItems.Add(new CartItem
                    {
                        Product = product,
                        Quantity = 1,
                        UnitPrice = product.Price
                    });
                }

                RefreshCart();
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                item.Quantity++;
                RefreshCart();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    CartItems.Remove(item);
                }

                RefreshCart();
            }
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

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            // TODO: команда реалізує SaleService.CreateSale(...) —
            // створення Sale/SaleItem, зменшення Stock товарів,
            // збереження через EF Core у SQLite та очищення кошика.
            MessageBox.Show(
                "Оформлення замовлення буде реалізовано бізнес-логікою команди.",
                "Магазин",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}
