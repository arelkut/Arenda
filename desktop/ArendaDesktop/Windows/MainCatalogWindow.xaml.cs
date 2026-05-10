using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ArendaDesktop.Helpers;
using ArendaDesktop.Models;
using ArendaDesktop.Services;

namespace ArendaDesktop.Windows
{
    public partial class MainCatalogWindow : Window
    {
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _total = 0;

        public MainCatalogWindow()
        {
            InitializeComponent();
            SetupHeader();
            LoadProperties();
        }

        private void SetupHeader()
        {
            UserEmailText.Text = AuthHelper.Email ?? "";
            if (AuthHelper.IsLandlord || AuthHelper.IsAdmin)
            {
                ProfileButton.Visibility = Visibility.Visible;
            }
            if (AuthHelper.IsAdmin)
            {
                AdminButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadProperties()
        {
            try
            {
                LoadingText.Visibility = Visibility.Visible;
                PropertiesPanel.Children.Clear();

                var search = SearchBox.Text.Trim();
                var city = CityFilter.Text.Trim();
                var typeItem = TypeFilter.SelectedItem as ComboBoxItem;
                var type = typeItem?.Content?.ToString();
                if (type == "Все") type = null;
                var roomsItem = RoomsFilter.SelectedItem as ComboBoxItem;
                var roomsStr = roomsItem?.Content?.ToString();
                int? rooms = null;
                if (roomsStr != null && roomsStr != "Все")
                {
                    if (int.TryParse(roomsStr.Replace("+", ""), out int r))
                        rooms = r;
                }
                double? maxPrice = null;
                if (double.TryParse(MaxPriceFilter.Text.Trim(), out double mp))
                    maxPrice = mp;

                var result = ApiService.GetProperties(
                    _currentPage, 12,
                    string.IsNullOrEmpty(search) ? null : search,
                    string.IsNullOrEmpty(city) ? null : city,
                    type, rooms, null, maxPrice
                );

                _total = result.Total;
                _totalPages = Math.Max(1, (int)Math.Ceiling(_total / 12.0));

                TotalText.Text = $"Найдено {_total} объявлений";
                PageText.Text = $"Страница {_currentPage} из {_totalPages}";
                PrevButton.IsEnabled = _currentPage > 1;
                NextButton.IsEnabled = _currentPage < _totalPages;

                foreach (var prop in result.Items)
                {
                    var card = CreatePropertyCard(prop);
                    PropertiesPanel.Children.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingText.Visibility = Visibility.Collapsed;
            }
        }

        private Border CreatePropertyCard(Property prop)
        {
            var card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 16, 16),
                Width = 280,
                Cursor = Cursors.Hand,
                Effect = new DropShadowEffect
                {
                    BlurRadius = 8, ShadowDepth = 1, Opacity = 0.08, Color = Colors.Black
                },
                Tag = prop.PropertyId
            };
            card.MouseLeftButtonUp += PropertyCard_Click;

            var stack = new StackPanel();

            // Photo
            var photoBorder = new Border
            {
                CornerRadius = new CornerRadius(12, 12, 0, 0),
                Height = 180,
                ClipToBounds = true,
                Background = new SolidColorBrush(Color.FromRgb(0xF1, 0xF5, 0xF9))
            };
            var photoGrid = new Grid();
            photoGrid.Children.Add(new TextBlock
            {
                Text = "Нет фото",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                FontSize = 14
            });
            if (!string.IsNullOrEmpty(prop.MainPhotoUrl))
            {
                try
                {
                    var img = new Image
                    {
                        Stretch = Stretch.UniformToFill,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(prop.MainPhotoUrl, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    img.Source = bmp;
                    photoGrid.Children.Add(img);
                }
                catch { }
            }
            photoBorder.Child = photoGrid;
            stack.Children.Add(photoBorder);

            // Info
            var info = new StackPanel { Margin = new Thickness(16, 12, 16, 16) };

            info.Children.Add(new TextBlock
            {
                Text = prop.Title,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            var location = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 6, 0, 0) };
            location.Children.Add(new TextBlock { Text = "📍 ", FontSize = 12 });
            location.Children.Add(new TextBlock
            {
                Text = $"{prop.City}, {prop.Address}",
                FontSize = 13,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxWidth = 220
            });
            info.Children.Add(location);

            // Tags
            var tags = new WrapPanel { Margin = new Thickness(0, 8, 0, 0) };
            tags.Children.Add(CreateTag(prop.AreaFormatted));
            if (prop.Rooms.HasValue)
                tags.Children.Add(CreateTag(prop.RoomsText));
            if (prop.Floor.HasValue)
                tags.Children.Add(CreateTag(prop.FloorInfo));
            info.Children.Add(tags);

            info.Children.Add(new TextBlock
            {
                Text = prop.PriceFormatted,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("AccentBrush"),
                Margin = new Thickness(0, 10, 0, 0)
            });

            stack.Children.Add(info);
            card.Child = stack;
            return card;
        }

        private Border CreateTag(string text)
        {
            return new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0xEF, 0xF6, 0xFF)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 2, 6, 2),
                Margin = new Thickness(0, 0, 6, 0),
                Child = new TextBlock
                {
                    Text = text,
                    FontSize = 12,
                    Foreground = (Brush)FindResource("AccentBrush")
                }
            };
        }

        private void PropertyCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is int propertyId)
            {
                var details = new PropertyDetailsWindow(propertyId);
                details.ShowDialog();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            LoadProperties();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _currentPage = 1;
                LoadProperties();
            }
        }

        private void ToggleFilters_Click(object sender, RoutedEventArgs e)
        {
            FiltersPanel.Visibility = FiltersPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            CityFilter.Text = "";
            TypeFilter.SelectedIndex = 0;
            RoomsFilter.SelectedIndex = 0;
            MaxPriceFilter.Text = "";
            _currentPage = 1;
            LoadProperties();
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadProperties();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadProperties();
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var realtor = new RealtorWindow();
            realtor.Show();
            Close();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var admin = new AdminWindow();
            admin.Show();
            Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthHelper.Clear();
            var login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}
