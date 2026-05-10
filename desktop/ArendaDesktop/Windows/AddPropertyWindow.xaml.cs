using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ArendaDesktop.Models;
using ArendaDesktop.Services;

namespace ArendaDesktop.Windows
{
    public partial class AddPropertyWindow : Window
    {
        public AddPropertyWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text.Trim();
            var city = CityBox.Text.Trim();
            var address = AddressBox.Text.Trim();
            var areaStr = AreaBox.Text.Trim();
            var priceStr = PriceBox.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(city) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(areaStr) ||
                string.IsNullOrEmpty(priceStr))
            {
                ShowError("Заполните все обязательные поля (отмечены *)");
                return;
            }

            if (!double.TryParse(areaStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double area))
            {
                ShowError("Площадь должна быть числом");
                return;
            }

            if (!double.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                ShowError("Цена должна быть числом");
                return;
            }

            var typeItem = PropertyTypeBox.SelectedItem as ComboBoxItem;
            var propertyType = typeItem?.Content?.ToString() ?? "Квартира";

            var data = new PropertyCreate
            {
                Title = title,
                Description = string.IsNullOrEmpty(DescriptionBox.Text.Trim()) ? null : DescriptionBox.Text.Trim(),
                Address = address,
                City = city,
                District = string.IsNullOrEmpty(DistrictBox.Text.Trim()) ? null : DistrictBox.Text.Trim(),
                PropertyType = propertyType,
                Area = area,
                Price = price
            };

            if (int.TryParse(RoomsBox.Text.Trim(), out int rooms))
                data.Rooms = rooms;
            if (int.TryParse(FloorBox.Text.Trim(), out int floor))
                data.Floor = floor;
            if (int.TryParse(TotalFloorsBox.Text.Trim(), out int totalFloors))
                data.TotalFloors = totalFloors;

            try
            {
                SubmitButton.IsEnabled = false;
                SubmitButton.Content = "Публикация...";

                ApiService.CreateProperty(data);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SubmitButton.IsEnabled = true;
                SubmitButton.Content = "Опубликовать объявление";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}
