using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    public partial class ProductEditWindow : Window
    {
        public Product Product { get; private set; }

        public ProductEditWindow(List<Category> categories, Product? productToEdit = null)
        {
            InitializeComponent();

            CategoryComboBox.ItemsSource = categories;

            if (productToEdit != null)
            {
                Product = productToEdit;

                HeaderTextBlock.Text = "Редагування товару";
                Title = "Редагування товару";

                NameTextBox.Text = productToEdit.Name;
                PriceTextBox.Text = productToEdit.Price.ToString(CultureInfo.InvariantCulture);
                StockTextBox.Text = productToEdit.Stock.ToString(CultureInfo.InvariantCulture);
                BarcodeTextBox.Text = productToEdit.Barcode ?? string.Empty;
                ImagePathTextBox.Text = productToEdit.ImagePath ?? string.Empty;
                IsAvailableCheckBox.IsChecked = productToEdit.IsAvailable;
                IsTopCheckBox.IsChecked = productToEdit.IsTop;
                IsNewCheckBox.IsChecked = productToEdit.IsNew;

                CategoryComboBox.SelectedValue = productToEdit.CategoryId;
            }
            else
            {
                Product = new Product();

                HeaderTextBlock.Text = "Новий товар";
                Title = "Новий товар";

                IsAvailableCheckBox.IsChecked = true;

                if (categories.Count > 0)
                {
                    CategoryComboBox.SelectedIndex = 0;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Введіть назву товару.");
                return;
            }

            if (CategoryComboBox.SelectedValue == null)
            {
                ShowError("Оберіть категорію товару.");
                return;
            }

            string priceText = PriceTextBox.Text.Trim().Replace(',', '.');

            if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
            {
                ShowError("Ціна має бути числом, наприклад 999.99");
                return;
            }

            if (price < 0)
            {
                ShowError("Ціна не може бути від'ємною.");
                return;
            }

            if (!int.TryParse(StockTextBox.Text.Trim(), out int stock) || stock < 0)
            {
                ShowError("Залишок має бути цілим невід'ємним числом.");
                return;
            }

            Product.Name = name;
            Product.Price = price;
            Product.Stock = stock;
            Product.CategoryId = (int)CategoryComboBox.SelectedValue;

            string barcode = BarcodeTextBox.Text.Trim();
            Product.Barcode = string.IsNullOrWhiteSpace(barcode) ? null : barcode;

            string imagePath = ImagePathTextBox.Text.Trim();
            Product.ImagePath = string.IsNullOrWhiteSpace(imagePath) ? null : imagePath;

            Product.IsAvailable = IsAvailableCheckBox.IsChecked == true;
            Product.IsTop = IsTopCheckBox.IsChecked == true;
            Product.IsNew = IsNewCheckBox.IsChecked == true;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}
