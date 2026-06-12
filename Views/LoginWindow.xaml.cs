using System.Windows;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: команда бізнес-логіки додасть перевірку логіну/пароля
            // через StoreDbContext (пошук користувача в базі та звірку пароля).
            // Зараз роль обирається перемикачем вручну — це лише демонстрація
            // поліморфізму (User -> Admin / Customer) та переходу між вікнами.

            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                ErrorTextBlock.Text = "Введіть логін.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            User currentUser = AdminRadio.IsChecked == true
                ? new Admin { Login = LoginTextBox.Text, FullName = "Адміністратор" }
                : new Customer { Login = LoginTextBox.Text, FullName = "Покупець" };

            // Поліморфний виклик: для Admin і Customer виконається власна реалізація.
            currentUser.ShowMenu();

            Window nextWindow = currentUser switch
            {
                Admin admin => new AdminWindow(admin),
                Customer customer => new MainWindow(customer),
                _ => new MainWindow(currentUser)
            };

            nextWindow.Show();
            Close();
        }
    }
}
