using System.Windows;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    public partial class CategoryEditWindow : Window
    {
        public Category Category { get; private set; }

        public CategoryEditWindow(Category? categoryToEdit = null)
        {
            InitializeComponent();

            if (categoryToEdit != null)
            {
                Category = categoryToEdit;

                HeaderTextBlock.Text = "Редагування категорії";
                Title = "Редагування категорії";

                NameTextBox.Text = categoryToEdit.Name;
                DescriptionTextBox.Text = categoryToEdit.Description ?? string.Empty;
                IsActiveCheckBox.IsChecked = categoryToEdit.IsActive;
            }
            else
            {
                Category = new Category();

                HeaderTextBlock.Text = "Нова категорія";
                Title = "Нова категорія";

                IsActiveCheckBox.IsChecked = true;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Введіть назву категорії.");
                return;
            }

            Category.Name = name;

            string description = DescriptionTextBox.Text.Trim();
            Category.Description = string.IsNullOrWhiteSpace(description) ? null : description;

            Category.IsActive = IsActiveCheckBox.IsChecked == true;

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
