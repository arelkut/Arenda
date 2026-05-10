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

        public PropertyDetailsWindow(int propertyId)
        {
            InitializeComponent();
            _propertyId = propertyId;
            LoadProperty();
        }

        private void LoadProperty()
        {
            try
            {
                var prop = ApiService.GetProperty(_propertyId);
                DisplayProperty(prop);
                LoadingText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LoadingText.Text = "Ошибка загрузки: " + ex.Message;
            }
        }

        private void DisplayProperty(Property prop)
        {
            Title = prop.Title + " — Недвижимость.РФ";
            TitleText.Text = prop.Title;
            AddressText.Text = $"{prop.City}, {prop.Address}" +
                (string.IsNullOrEmpty(prop.District) ? "" : $" (р-н {prop.District})");
            PriceText.Text = prop.PriceFormatted;

            // Main photo
            if (!string.IsNullOrEmpty(prop.MainPhotoUrl))
            {
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(prop.MainPhotoUrl, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    MainPhotoImage.Source = bmp;
                }
                catch { }
            }

            // Thumbnails
            if (prop.Media != null && prop.Media.Count > 1)
            {
                foreach (var media in prop.Media)
                {
                    if (media.IsMain) continue;
                    try
                    {
                        var thumb = new Border
                        {
                            Width = 80, Height = 60,
                            CornerRadius = new CornerRadius(8),
                            ClipToBounds = true,
                            Margin = new Thickness(0, 0, 8, 0),
                            Background = new SolidColorBrush(Color.FromRgb(0xF1, 0xF5, 0xF9))
                        };
                        var img = new Image { Stretch = Stretch.UniformToFill };
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(media.FilePath, UriKind.Absolute);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        img.Source = bmp;
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
