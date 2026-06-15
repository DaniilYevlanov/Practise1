using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MagazinWPF.Data;
using MagazinWPF.Models;

namespace MagazinWPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            EnsureDatabaseCreated();
        }

        private static void EnsureDatabaseCreated()
        {
            using var db = new StoreDbContext();
            db.Database.EnsureCreated();
        }

        private void TabLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility    = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
            HideMessage();

            TabLoginButton.Background    = (Brush)FindResource("PrimaryBrush");
            TabLoginButton.Foreground    = Brushes.White;
            TabRegisterButton.Background = (Brush)FindResource("BackgroundBrush");
            TabRegisterButton.Foreground = (Brush)FindResource("MutedTextBrush");
        }

        private void TabRegister_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility    = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
            HideMessage();

            TabRegisterButton.Background = (Brush)FindResource("PrimaryBrush");
            TabRegisterButton.Foreground = Brushes.White;
            TabLoginButton.Background    = (Brush)FindResource("BackgroundBrush");
            TabLoginButton.Foreground    = (Brush)FindResource("MutedTextBrush");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login    = LoginTextBox.Text.Trim();
            string password = LoginPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введіть логін і пароль.");
                return;
            }

            using var db = new StoreDbContext();
            var account = db.Users.FirstOrDefault(u => u.Login == login && u.IsActive);

            if (account == null || !account.VerifyPassword(password))
            {
                ShowError("Невірний логін або пароль.");
                return;
            }

            User currentUser = account.ToUser();
            currentUser.ShowMenu();

            Window nextWindow = currentUser switch
            {
                Admin    admin    => new AdminWindow(admin),
                Customer customer => new MainWindow(customer),
                _                => new MainWindow(currentUser)
            };

            nextWindow.Show();
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName        = RegFullNameTextBox.Text.Trim();
            string login           = RegLoginTextBox.Text.Trim();
            string password        = RegPasswordBox.Password;
            string confirmPassword = RegConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(login)    ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowError("Заповніть усі поля.");
                return;
            }

            if (login.Length < 3)
            {
                ShowError("Логін має містити мінімум 3 символи.");
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Пароль має містити мінімум 6 символів.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Паролі не збігаються.");
                return;
            }

            using var db = new StoreDbContext();

            if (db.Users.Any(u => u.Login == login))
            {
                ShowError("Користувач із таким логіном вже існує.");
                return;
            }

            var newAccount = new UserAccount
            {
                Login        = login,
                PasswordHash = UserAccount.HashPassword(password),
                FullName     = fullName,
                Role         = "Customer",
                IsActive     = true,
                CreatedAt    = DateTime.Now
            };

            db.Users.Add(newAccount);
            db.SaveChanges();

            RegFullNameTextBox.Text  = string.Empty;
            RegLoginTextBox.Text     = string.Empty;
            RegPasswordBox.Password  = string.Empty;
            RegConfirmPasswordBox.Password = string.Empty;

            ShowSuccess("Реєстрацію успішно завершено! Тепер увійдіть.");
            TabLogin_Click(sender, e);
            LoginTextBox.Text = login;
        }

        private void ShowError(string message)
        {
            MessageTextBlock.Text       = message;
            MessageTextBlock.Foreground = (Brush)FindResource("DangerBrush");
            MessageTextBlock.Visibility = Visibility.Visible;
        }

        private void ShowSuccess(string message)
        {
            MessageTextBlock.Text       = message;
            MessageTextBlock.Foreground = (Brush)FindResource("PrimaryBrush");
            MessageTextBlock.Visibility = Visibility.Visible;
        }

        private void HideMessage()
        {
            MessageTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
