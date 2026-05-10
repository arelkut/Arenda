using System;
using System.Windows;
using System.Windows.Input;
using ArendaDesktop.Helpers;
using ArendaDesktop.Services;

namespace ArendaDesktop.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            PasswordBox.KeyDown += (s, e) => { if (e.Key == Key.Enter) LoginButton_Click(s, e); };
            EmailBox.KeyDown += (s, e) => { if (e.Key == Key.Enter) PasswordBox.Focus(); };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Заполните все поля");
                return;
            }

            try
            {
                LoginButton.IsEnabled = false;
                LoginButton.Content = "Вход...";

                var response = ApiService.Login(email, password);
                AuthHelper.SetAuth(response);

                OpenMainWindow();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Войти";
            }
        }

        private void OpenMainWindow()
        {
            if (AuthHelper.IsAdmin)
            {
                var admin = new AdminWindow();
                admin.Show();
            }
            else if (AuthHelper.IsLandlord)
            {
                var realtor = new RealtorWindow();
                realtor.Show();
            }
            else
            {
                var catalog = new MainCatalogWindow();
                catalog.Show();
            }
            Close();
        }

        private void RegisterLink_Click(object sender, MouseButtonEventArgs e)
        {
            var register = new RegisterWindow();
            register.Show();
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}
