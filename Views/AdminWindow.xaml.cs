using System;
using System.Collections.ObjectModel;
using System.Windows;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    /// <summary>
    /// Панель адміністратора: товари, категорії, продажі.
    /// Наразі реалізовано лише зовнішній вигляд (вкладки, таблиці, кнопки) —
    /// реальні операції додавання/редагування/видалення та роботу з базою
    /// (EF Core / SQLite, ProductService, SaleService тощо) додасть команда бізнес-логіки.
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly User _currentUser;

        public ObservableCollection<Product> Products { get; } = new();

        public ObservableCollection<Category> Categories { get; } = new();

        public ObservableCollection<Sale> Sales { get; } = new();

        public AdminWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            UserNameTextBlock.Text = $"{currentUser.FullName} ({currentUser.Role})";

            // Демонстраційні дані для зовнішнього вигляду.
            // TODO: команда замінить це на завантаження даних з бази
            // через StoreDbContext / Services (ProductService, CartService, SaleService).
            LoadDemoData();

            ProductsItemsControl.ItemsSource = Products;
            CategoriesItemsControl.ItemsSource = Categories;
            SalesDataGrid.ItemsSource = Sales;
        }

        private void LoadDemoData()
        {
            Categories.Add(new Category { Id = 1, Name = "Смартфони", Description = "Мобільні телефони", IsActive = true });
            Categories.Add(new Category { Id = 2, Name = "Ноутбуки", Description = "Портативні комп'ютери", IsActive = true });
            Categories.Add(new Category { Id = 3, Name = "Периферія", Description = "Миші, клавіатури тощо", IsActive = true });

            Products.Add(new Product { Id = 1, Name = "Samsung A26", Price = 9899, Stock = 10, CategoryId = 1 });
            Products.Add(new Product { Id = 2, Name = "Xiaomi 15", Price = 36999, Stock = 4, CategoryId = 1 });
            Products.Add(new Product { Id = 3, Name = "Lenovo ThinkPad", Price = 35000, Stock = 5, CategoryId = 2 });
            Products.Add(new Product { Id = 4, Name = "Logitech Mouse", Price = 800, Stock = 20, CategoryId = 3 });

            Sales.Add(new Sale { Id = 1, SaleDate = DateTime.Now, TotalAmount = 9899, CashierName = "Демо" });
        }

        // --- Товари ---

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // TODO: відкрити форму додавання товару та викликати ProductService.AddProduct(...).
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            // TODO: sender -> Button -> Tag містить Product картки, на якій натиснули "Редагувати".
            // Відкрити форму редагування для цього товару через ProductService.
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            // TODO: sender -> Button -> Tag містить Product картки, на якій натиснули "Видалити".
            // Видалити товар через ProductService.
        }

        // --- Категорії ---

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            // TODO: відкрити форму додавання категорії.
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            // TODO: sender -> Button -> Tag містить Category картки, на якій натиснули "Редагувати".
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            // TODO: sender -> Button -> Tag містить Category картки, на якій натиснули "Видалити".
        }

        // --- Продажі ---

        private void RefreshSales_Click(object sender, RoutedEventArgs e)
        {
            // TODO: перезавантажити список продажів з бази через SaleService.
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}
