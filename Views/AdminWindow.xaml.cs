using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MagazinWPF.Models;
using MagazinWPF.Services;

namespace MagazinWPF.Views
{
    public partial class AdminWindow : Window
    {
        private readonly User _currentUser;

        private readonly ProductService _productService = new();
        private readonly CategoryService _categoryService = new();
        private readonly SaleService _saleService = new();

        public ObservableCollection<Product> Products { get; } = new();

        public ObservableCollection<Category> Categories { get; } = new();

        public ObservableCollection<Sale> Sales { get; } = new();

        public AdminWindow(User currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;
            UserNameTextBlock.Text = $"{currentUser.FullName} ({currentUser.Role})";

            ProductsItemsControl.ItemsSource = Products;
            CategoriesItemsControl.ItemsSource = Categories;
            SalesDataGrid.ItemsSource = Sales;

            LoadCategories();
            LoadProducts();
            LoadSales();
        }

        private void LoadCategories()
        {
            Categories.Clear();

            foreach (var category in _categoryService.GetAll())
            {
                Categories.Add(category);
            }
        }

        private void LoadProducts()
        {
            Products.Clear();

            foreach (var product in _productService.GetAll())
            {
                Products.Add(product);
            }
        }

        private void LoadSales()
        {
            Sales.Clear();

            foreach (var sale in _saleService.GetAll())
            {
                Sales.Add(sale);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (Categories.Count == 0)
            {
                MessageBox.Show(
                    "Спочатку додайте хоча б одну категорію.",
                    "Магазин",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var window = new ProductEditWindow(Categories.ToList())
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                _productService.Add(window.Product);
                LoadProducts();
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Product product)
            {
                return;
            }

            var window = new ProductEditWindow(Categories.ToList(), product)
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                _productService.Update(window.Product);
                LoadProducts();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Product product)
            {
                return;
            }

            var result = MessageBox.Show(
                $"Видалити товар \"{product.Name}\"?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            bool deleted = _productService.Delete(product.Id);

            if (!deleted)
            {
                MessageBox.Show(
                    "Неможливо видалити цей товар: він уже фігурує в продажах або кошиках.\n" +
                    "Можна позначити його як недоступний для продажу замість видалення.",
                    "Магазин",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            LoadProducts();
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var window = new CategoryEditWindow
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                _categoryService.Add(window.Category);
                LoadCategories();
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Category category)
            {
                return;
            }

            var window = new CategoryEditWindow(category)
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                _categoryService.Update(window.Category);
                LoadCategories();
                LoadProducts();
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Category category)
            {
                return;
            }

            var result = MessageBox.Show(
                $"Видалити категорію \"{category.Name}\"?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            bool deleted = _categoryService.Delete(category.Id);

            if (!deleted)
            {
                MessageBox.Show(
                    "Неможливо видалити цю категорію: у ній ще є товари.\n" +
                    "Спочатку перенесіть або видаліть товари цієї категорії.",
                    "Магазин",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            LoadCategories();
        }

        private void RefreshSales_Click(object sender, RoutedEventArgs e)
        {
            LoadSales();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}
