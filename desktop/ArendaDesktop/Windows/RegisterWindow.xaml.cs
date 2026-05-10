using System;
using System.Windows;
using System.Windows.Input;
using ArendaDesktop.Helpers;
using ArendaDesktop.Services;

namespace ArendaDesktop.Windows
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text.Trim();
            var password = PasswordBox.Password;
            var confirmPwd = ConfirmPasswordBox.Password;
            var firstName = FirstNameBox.Text.Trim();
            var lastName = LastNameBox.Text.Trim();
            var phone = PhoneBox.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Заполните обязательные поля (Email и Пароль)");
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Пароль должен содержать минимум 6 символов");
                return;
            }

            if (password != confirmPwd)
            {
                ShowError("Пароли не совпадают");
                return;
            }

            try
            {
                RegisterButton.IsEnabled = false;
                RegisterButton.Content = "Регистрация...";

                var response = ApiService.Register(
                    email, password,
                    string.IsNullOrEmpty(firstName) ? null : firstName,
                    string.IsNullOrEmpty(lastName) ? null : lastName,
                    string.IsNullOrEmpty(phone) ? null : phone
                );
                AuthHelper.SetAuth(response);

                var catalog = new MainCatalogWindow();
                catalog.Show();
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                RegisterButton.Content = "Зарегистрироваться";
            }
        }

        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}
