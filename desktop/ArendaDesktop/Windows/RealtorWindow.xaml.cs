using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ArendaDesktop.Helpers;
using ArendaDesktop.Models;
using ArendaDesktop.Services;

namespace ArendaDesktop.Windows
{
    public partial class RealtorWindow : Window
    {
        public RealtorWindow()
        {
            InitializeComponent();
            UserEmailText.Text = AuthHelper.Email ?? "";
            LoadMyProperties();
        }

        private void LoadMyProperties()
        {
            try
            {
                LoadingText.Visibility = Visibility.Visible;
                PropertiesList.Children.Clear();

                var result = ApiService.GetProperties(1, 50);
                var myProps = new System.Collections.Generic.List<Property>();
                foreach (var p in result.Items)
                {
                    if (p.LandlordId == AuthHelper.UserId)
                        myProps.Add(p);
                }

                if (myProps.Count == 0)
                {
                    PropertiesList.Children.Add(new TextBlock
                    {
                        Text = "У вас пока нет объявлений. Нажмите «Добавить объявление».",
                        FontSize = 15,
                        Foreground = (Brush)FindResource("TextSecondaryBrush"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 40, 0, 0)
                    });
                }
                else
                {
                    foreach (var prop in myProps)
                    {
                        PropertiesList.Children.Add(CreatePropertyRow(prop));
                    }
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

        private Border CreatePropertyRow(Property prop)
        {
            var card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 0, 12),
                Padding = new Thickness(16),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 6, ShadowDepth = 1, Opacity = 0.06, Color = Colors.Black
                }
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Photo
            var photoBorder = new Border
            {
                Width = 80, Height = 60,
                CornerRadius = new CornerRadius(8),
                ClipToBounds = true,
                Background = new SolidColorBrush(Color.FromRgb(0xF1, 0xF5, 0xF9)),
                Margin = new Thickness(0, 0, 12, 0)
            };
            if (!string.IsNullOrEmpty(prop.MainPhotoUrl))
            {
                try
                {
                    var img = new Image { Stretch = Stretch.UniformToFill };
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(prop.MainPhotoUrl, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    img.Source = bmp;
                    photoBorder.Child = img;
                }
                catch { }
            }
            Grid.SetColumn(photoBorder, 0);
            grid.Children.Add(photoBorder);

            // Info
            var info = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            info.Children.Add(new TextBlock
            {
                Text = prop.Title,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("TextPrimaryBrush")
            });
            info.Children.Add(new TextBlock
            {
                Text = $"{prop.City}, {prop.Address}",
                FontSize = 13,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 2, 0, 0)
            });

            var statusInfo = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
            statusInfo.Children.Add(new TextBlock
            {
                Text = prop.PriceFormatted,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("AccentBrush"),
                Margin = new Thickness(0, 0, 12, 0)
            });

            Brush statusBrush;
            switch (prop.StatusId)
            {
                case 2: statusBrush = (Brush)FindResource("SuccessBrush"); break;
                case 3: statusBrush = (Brush)FindResource("DangerBrush"); break;
                default: statusBrush = (Brush)FindResource("WarningBrush"); break;
            }
            var statusBorder = new Border
            {
                Background = statusBrush,
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 2, 8, 2),
                Child = new TextBlock
                {
                    Text = prop.StatusText,
                    FontSize = 12,
                    Foreground = Brushes.White
                }
            };
            statusInfo.Children.Add(statusBorder);
            info.Children.Add(statusInfo);

            Grid.SetColumn(info, 1);
            grid.Children.Add(info);

            // Actions
            var actions = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var viewBtn = new Button
            {
                Content = "Просмотр",
                Style = (Style)FindResource("SecondaryButton"),
                Padding = new Thickness(12, 6, 12, 6),
                FontSize = 13,
                Margin = new Thickness(0, 0, 8, 0),
                Tag = prop.PropertyId
            };
            viewBtn.Click += (s, e) =>
            {
                var details = new PropertyDetailsWindow((int)((Button)s).Tag);
                details.ShowDialog();
            };
            actions.Children.Add(viewBtn);

            var deleteBtn = new Button
            {
                Content = "Удалить",
                Style = (Style)FindResource("DangerButton"),
                Padding = new Thickness(12, 6, 12, 6),
                FontSize = 13,
                Tag = prop.PropertyId
            };
            deleteBtn.Click += DeleteProperty_Click;
            actions.Children.Add(deleteBtn);

            Grid.SetColumn(actions, 2);
            grid.Children.Add(actions);

            card.Child = grid;
            return card;
        }

        private void DeleteProperty_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var propertyId = (int)btn.Tag;

            var result = MessageBox.Show("Вы уверены, что хотите удалить это объявление?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ApiService.DeleteProperty(propertyId);
                    ShowMessage("Объявление удалено", true);
                    LoadMyProperties();
                }
                catch (Exception ex)
                {
                    ShowMessage("Ошибка удаления: " + ex.Message, false);
                }
            }
        }

        private void AddProperty_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddPropertyWindow();
            var result = addWindow.ShowDialog();
            if (result == true)
            {
                ShowMessage("Объявление создано и отправлено на модерацию", true);
                LoadMyProperties();
            }
        }

        private void ShowMessage(string text, bool isSuccess)
        {
            MessageText.Text = text;
            MessageText.Foreground = isSuccess ? Brushes.White : Brushes.White;
            MessageBorder.Background = isSuccess
                ? (Brush)FindResource("SuccessBrush")
                : (Brush)FindResource("DangerBrush");
            MessageBorder.Visibility = Visibility.Visible;
        }

        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            var catalog = new MainCatalogWindow();
            catalog.Show();
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
