using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ArendaDesktop.Models;
using ArendaDesktop.Services;

namespace ArendaDesktop.Windows
{
    public partial class PropertyDetailsWindow : Window
    {
        private readonly int _propertyId;

        private static readonly string[] StaticImages = new string[]
        {
            "pack://application:,,,/Images/property1.jpg",
            "pack://application:,,,/Images/property2.jpg",
            "pack://application:,,,/Images/property3.jpg",
            "pack://application:,,,/Images/property4.jpg",
            "pack://application:,,,/Images/property5.jpg",
            "pack://application:,,,/Images/property6.jpg"
        };

        public PropertyDetailsWindow(int propertyId)
        {
            InitializeComponent();
            _propertyId = propertyId;
            LoadProperty();
        }

        private static BitmapImage GetStaticImage(int id)
        {
            var index = Math.Abs(id) % StaticImages.Length;
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(StaticImages[index], UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        private void LoadProperty()
        {
            try
            {
                var prop = ApiService.GetProperty(_propertyId);
                DisplayProperty(prop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayProperty(Property prop)
        {
            Title = prop.Title + " — Недвижимость.РФ";
            TitleText.Text = prop.Title;
            AddressText.Text = $"{prop.City}, {prop.Address}" +
                (string.IsNullOrEmpty(prop.District) ? "" : $" (р-н {prop.District})");
            PriceText.Text = prop.PriceFormatted;

            // Main photo (static)
            try
            {
                MainPhotoImage.Source = GetStaticImage(prop.PropertyId);
            }
            catch { }

            // Thumbnails (static — different images for variety)
            if (prop.Media != null && prop.Media.Count > 1)
            {
                for (int i = 1; i < prop.Media.Count && i <= 4; i++)
                {
                    try
                    {
                        var thumb = new Border
                        {
                            Width = 80, Height = 60,
                            CornerRadius = new CornerRadius(8),
                            ClipToBounds = true,
                            Margin = new Thickness(0, 0, 8, 0),
                            Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0))
                        };
                        var img = new Image
                        {
                            Source = GetStaticImage(prop.PropertyId + i),
                            Stretch = Stretch.UniformToFill
                        };
                        thumb.Child = img;
                        ThumbnailsPanel.Children.Add(thumb);
                    }
                    catch { }
                }
            }

            // Features
            int row = 0;
            AddFeature("Тип", prop.PropertyType, ref row);
            AddFeature("Площадь", prop.AreaFormatted, ref row);
            if (prop.Rooms.HasValue) AddFeature("Комнат", prop.Rooms.Value.ToString(), ref row);
            if (prop.Floor.HasValue) AddFeature("Этаж", prop.FloorInfo, ref row);
            AddFeature("Город", prop.City, ref row);
            if (!string.IsNullOrEmpty(prop.District)) AddFeature("Район", prop.District, ref row);

            // Description
            if (!string.IsNullOrEmpty(prop.Description))
            {
                DescriptionText.Text = prop.Description;
                DescriptionBorder.Visibility = Visibility.Visible;
            }
        }

        private void AddFeature(string label, string value, ref int row)
        {
            FeaturesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var lbl = new TextBlock
            {
                Text = label,
                FontSize = 13,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 0, 0, 8)
            };
            Grid.SetRow(lbl, row);
            Grid.SetColumn(lbl, 0);
            FeaturesGrid.Children.Add(lbl);

            var val = new TextBlock
            {
                Text = value,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
                Margin = new Thickness(0, 0, 0, 8)
            };
            Grid.SetRow(val, row);
            Grid.SetColumn(val, 1);
            FeaturesGrid.Children.Add(val);

            row++;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
