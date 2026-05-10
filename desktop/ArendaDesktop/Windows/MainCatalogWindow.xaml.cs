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

        private static readonly string[] StaticImages = new string[]
        {
            "pack://application:,,,/Images/property1.jpg",
            "pack://application:,,,/Images/property2.jpg",
            "pack://application:,,,/Images/property3.jpg",
            "pack://application:,,,/Images/property4.jpg",
            "pack://application:,,,/Images/property5.jpg",
            "pack://application:,,,/Images/property6.jpg"
        };

        public MainCatalogWindow()
        {
            InitializeComponent();
            SetupHeader();
            LoadProperties();
        }

        private void SetupHeader()
        {
            UserEmailText.Text = AuthHelper.Email ?? "";
            if (AuthHelper.IsLandlord)
            {
                MyPropertiesButton.Visibility = Visibility.Visible;
            }
            if (AuthHelper.IsAdmin)
            {
                AdminButton.Visibility = Visibility.Visible;
            }
        }

        private static BitmapImage GetStaticImage(int propertyId)
        {
            var index = Math.Abs(propertyId) % StaticImages.Length;
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(StaticImages[index], UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.DecodePixelWidth = 400;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
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
                if (type == "Все типы") type = null;
                var roomsItem = RoomsFilter.SelectedItem as ComboBoxItem;
                var roomsStr = roomsItem?.Content?.ToString();
                int? rooms = null;
                if (roomsStr != null && roomsStr != "Любое")
                {
                    if (int.TryParse(roomsStr.Replace("+", ""), out int r))
                        rooms = r;
                }
                double? minPrice = null;
                if (double.TryParse(MinPriceFilter.Text.Trim(), out double mnp))
                    minPrice = mnp;
                double? maxPrice = null;
                if (double.TryParse(MaxPriceFilter.Text.Trim(), out double mxp))
                    maxPrice = mxp;

                var result = ApiService.GetProperties(
                    _currentPage, 12,
                    string.IsNullOrEmpty(search) ? null : search,
                    string.IsNullOrEmpty(city) ? null : city,
                    type, rooms, minPrice, maxPrice
                );

                _total = result.Total;
                _totalPages = Math.Max(1, (int)Math.Ceiling(_total / 12.0));

                TotalText.Text = $"Найдено объектов: {_total}";
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
                Width = 270,
                Cursor = Cursors.Hand,
                Effect = new DropShadowEffect
                {
                    BlurRadius = 8,
                    ShadowDepth = 1,
                    Opacity = 0.06,
                    Color = Colors.Black
                },
                Tag = prop.PropertyId
            };
            card.MouseLeftButtonUp += PropertyCard_Click;

            var stack = new StackPanel();

            var photoBorder = new Border
            {
                CornerRadius = new CornerRadius(12, 12, 0, 0),
                Height = 180,
                ClipToBounds = true,
                Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0))
            };

            var photoGrid = new Grid();
            try
            {
                var img = new Image
                {
                    Source = GetStaticImage(prop.PropertyId),
                    Stretch = Stretch.UniformToFill,
                    VerticalAlignment = VerticalAlignment.Center
                };
                photoGrid.Children.Add(img);
            }
            catch
            {
                photoGrid.Children.Add(new TextBlock
                {
                    Text = "Нет фото",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = (Brush)FindResource("TextSecondaryBrush"),
                    FontSize = 14
                });
            }

            photoBorder.Child = photoGrid;
            stack.Children.Add(photoBorder);

            var info = new StackPanel { Margin = new Thickness(16, 12, 16, 16) };

            info.Children.Add(new TextBlock
            {
                Text = prop.Title,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            var location = prop.City;
            if (!string.IsNullOrEmpty(prop.District))
                location += $", {prop.District}";

            info.Children.Add(new TextBlock
            {
                Text = location,
                FontSize = 13,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                TextTrimming = TextTrimming.CharacterEllipsis,
                Margin = new Thickness(0, 4, 0, 0)
            });

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
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
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
                Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 3, 8, 3),
                Margin = new Thickness(0, 0, 6, 0),
                Child = new TextBlock
                {
                    Text = text,
                    FontSize = 12,
                    Foreground = (Brush)FindResource("TextPrimaryBrush")
                }
            };
        }

        private void PropertyCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is int propertyId)
            {
                var details = new PropertyDetailsWindow(propertyId);
                details.ShowDialog();
                LoadProperties();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            LoadProperties();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            CityFilter.Text = "";
            TypeFilter.SelectedIndex = 0;
            RoomsFilter.SelectedIndex = 0;
            MinPriceFilter.Text = "";
            MaxPriceFilter.Text = "";
            _currentPage = 1;
            LoadProperties();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadProperties();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadProperties();
            }
        }

        private void MyPropertiesButton_Click(object sender, RoutedEventArgs e)
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
