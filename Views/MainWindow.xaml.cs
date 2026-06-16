using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MagazinWPF.Models;
using MagazinWPF.Services;

namespace MagazinWPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        private readonly ProductService _productService = new();
        private readonly CategoryService _categoryService = new();
        private readonly SaleService _saleService = new();

        private int? _selectedCategoryId;

        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<CartItem> CartItems { get; } = new();

        public ObservableCollection<CategoryFilterItem> CategoryFilters { get; } = new();

        public MainWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            UserNameTextBlock.Text = $"{currentUser.FullName} ({currentUser.Role})";

            ProductsItemsControl.ItemsSource = Products;
            CartItemsControl.ItemsSource = CartItems;

            CategoriesListBox.ItemsSource = CategoryFilters;
            CategoriesListBox.DisplayMemberPath = "Name";

            LoadCategories();
            LoadProducts();
            RefreshCart();

            DataEvents.ProductsChanged += OnProductsChanged;
            DataEvents.CategoriesChanged += OnCategoriesChanged;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            DataEvents.ProductsChanged -= OnProductsChanged;
            DataEvents.CategoriesChanged -= OnCategoriesChanged;
        }

        private void OnProductsChanged()
        {
            Dispatcher.Invoke(LoadProducts);
        }

        private void OnCategoriesChanged()
        {
            Dispatcher.Invoke(() =>
            {
                LoadCategories();
                LoadProducts();
            });
        }

        private void LoadCategories()
        {
            int? previouslySelected = _selectedCategoryId;

            CategoryFilters.Clear();
            CategoryFilters.Add(new CategoryFilterItem(null, "Усі товари"));

            foreach (var category in _categoryService.GetAll().Where(c => c.IsActive))
            {
                CategoryFilters.Add(new CategoryFilterItem(category.Id, category.Name));
            }

            var match = CategoryFilters.FirstOrDefault(c => c.Id == previouslySelected);
            CategoriesListBox.SelectedItem = match ?? CategoryFilters[0];
        }

        private void LoadProducts()
        {
            Products.Clear();

            var items = _selectedCategoryId == null
                ? _productService.GetAll()
                : _productService.GetByCategory(_selectedCategoryId.Value);

            foreach (var product in items.Where(p => p.IsAvailable))
            {
                Products.Add(product);
            }
        }

        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesListBox.SelectedItem is CategoryFilterItem item)
            {
                _selectedCategoryId = item.Id;
                LoadProducts();
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Product product)
                return;

            var existing = CartItems.FirstOrDefault(i => i.Product?.Id == product.Id);
            int currentQuantity = existing?.Quantity ?? 0;

            if (currentQuantity + 1 > product.Stock)
            {
                MessageBox.Show(
                    $"У наявності лише {product.Stock} шт. товару «{product.Name}».",
                    "Недостатньо товару",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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
                MessageBox.Show(
                    $"У наявності лише {stock} шт. товару «{item.Product?.Name}».",
                    "Недостатньо товару",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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

            Sale sale;

            try
            {
                sale = _saleService.Checkout(CartItems, _currentUser.FullName);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Недостатньо товару",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadProducts();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка при оформленні замовлення: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CartItems.Clear();
            RefreshCart();
            LoadProducts();

            var receiptWindow = new ReceiptWindow(sale) { Owner = this };
            receiptWindow.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}