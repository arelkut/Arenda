using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class AdminWindow : Window
    {
        private string _activeTab = "properties";
        private List<Property> _allProperties = new List<Property>();

        public AdminWindow()
        {
            InitializeComponent();
            SetupProfile();
            LoadAllData();
            ShowProperties();
        }

        private void SetupProfile()
        {
            AdminNameText.Text = AuthHelper.Email ?? "Администратор";
        }

        private void LoadAllData()
        {
            try
            {
                var result = ApiService.GetAllPropertiesForAdmin(1, 100);
                _allProperties = result.Items;
                TotalPropsText.Text = result.Total.ToString();
                PendingPropsText.Text = _allProperties.Count(p => p.StatusId == 1).ToString();
                ActivePropsText.Text = _allProperties.Count(p => p.StatusId == 2).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HighlightNav(string tab)
        {
            _activeTab = tab;
            NavProperties.Background = tab == "properties"
                ? new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x3F)) : Brushes.Transparent;
            NavProperties.Foreground = tab == "properties" ? Brushes.White
                : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
            NavUsers.Background = tab == "users"
                ? new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x3F)) : Brushes.Transparent;
            NavUsers.Foreground = tab == "users" ? Brushes.White
                : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
            NavSettings.Background = tab == "settings"
                ? new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x3F)) : Brushes.Transparent;
            NavSettings.Foreground = tab == "settings" ? Brushes.White
                : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

            PropertiesTab.Visibility = tab == "properties" ? Visibility.Visible : Visibility.Collapsed;
            UsersTab.Visibility = tab == "users" ? Visibility.Visible : Visibility.Collapsed;
            SettingsTab.Visibility = tab == "settings" ? Visibility.Visible : Visibility.Collapsed;
        }

        // ===== Properties Tab =====
        private void ShowProperties()
        {
            HighlightNav("properties");
            RenderPropertiesTable();
        }

        private void RenderPropertiesTable()
        {
            PropertiesList.Children.Clear();
            var search = PropsSearchBox.Text.Trim().ToLower();
            var filtered = _allProperties.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(p =>
                    p.Title.ToLower().Contains(search) ||
                    p.City.ToLower().Contains(search));
            }

            var header = CreateTableRow(
                new[] { "", "Название", "Город", "Цена", "Статус", "Действия" }, true);
            PropertiesList.Children.Add(header);

            foreach (var prop in filtered)
            {
                PropertiesList.Children.Add(CreatePropertyRow(prop));
            }
        }

        private Border CreatePropertyRow(Property prop)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = (Brush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(16, 10, 16, 10)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Thumbnail
            var thumbBorder = new Border
            {
                Width = 36, Height = 36, CornerRadius = new CornerRadius(6),
                ClipToBounds = true, Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0)),
                VerticalAlignment = VerticalAlignment.Center
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
                    bmp.DecodePixelWidth = 72;
                    bmp.EndInit();
                    img.Source = bmp;
                    thumbBorder.Child = img;
                }
                catch { }
            }
            Grid.SetColumn(thumbBorder, 0);
            grid.Children.Add(thumbBorder);

            AddCell(grid, prop.Title, 1, FontWeights.SemiBold);
            AddCell(grid, prop.City, 2);
            AddCell(grid, prop.PriceFormatted, 3);

            // Status badge
            Brush statusBg; Brush statusFg;
            switch (prop.StatusId)
            {
                case 2:
                    statusBg = new SolidColorBrush(Color.FromRgb(0xDC, 0xFC, 0xE7));
                    statusFg = (Brush)FindResource("SuccessBrush"); break;
                case 3:
                    statusBg = new SolidColorBrush(Color.FromRgb(0xFE, 0xF2, 0xF2));
                    statusFg = (Brush)FindResource("DangerBrush"); break;
                default:
                    statusBg = new SolidColorBrush(Color.FromRgb(0xFE, 0xF9, 0xC3));
                    statusFg = new SolidColorBrush(Color.FromRgb(0xCA, 0x8A, 0x04)); break;
            }
            var statusBorder = new Border
            {
                CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 2, 8, 2),
                Background = statusBg, HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock { Text = prop.StatusText, FontSize = 12, Foreground = statusFg }
            };
            Grid.SetColumn(statusBorder, 4);
            grid.Children.Add(statusBorder);

            // Actions
            var actions = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            if (prop.StatusId == 1)
            {
                var approveBtn = new Button
                {
                    Content = "Одобрить", Style = (Style)FindResource("SuccessButton"),
                    Padding = new Thickness(8, 4, 8, 4), FontSize = 12,
                    Margin = new Thickness(0, 0, 4, 0), Tag = prop.PropertyId
                };
                approveBtn.Click += ApproveProperty_Click;
                actions.Children.Add(approveBtn);

                var rejectBtn = new Button
                {
                    Content = "Отклонить", Style = (Style)FindResource("DangerButton"),
                    Padding = new Thickness(8, 4, 8, 4), FontSize = 12,
                    Tag = prop.PropertyId
                };
                rejectBtn.Click += RejectProperty_Click;
                actions.Children.Add(rejectBtn);
            }
            else
            {
                var viewBtn = new Button
                {
                    Content = "Просмотр", Style = (Style)FindResource("SecondaryButton"),
                    Padding = new Thickness(8, 4, 8, 4), FontSize = 12, Tag = prop.PropertyId
                };
                viewBtn.Click += (s, e) =>
                {
                    var d = new PropertyDetailsWindow((int)((Button)s).Tag);
                    d.ShowDialog();
                };
                actions.Children.Add(viewBtn);
            }
            Grid.SetColumn(actions, 5);
            grid.Children.Add(actions);

            border.Child = grid;
            return border;
        }

        // ===== Users Tab =====
        private void ShowUsers()
        {
            HighlightNav("users");
            RenderUsersTable();
        }

        private void RenderUsersTable()
        {
            UsersList.Children.Clear();
            var users = GetDemoUsers();
            var search = UsersSearchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Email.ToLower().Contains(search)).ToList();
            }

            var header = CreateTableRow(
                new[] { "Пользователь", "Роль", "Контакты", "Статус" }, true);
            UsersList.Children.Add(header);

            foreach (var user in users)
            {
                UsersList.Children.Add(CreateUserRow(user));
            }
        }

        private List<UserInfo> GetDemoUsers()
        {
            return new List<UserInfo>
            {
                new UserInfo { UserId = 1, Email = "admin@example.com", Phone = "+79001234567",
                    IsActive = true, Roles = new List<string> { "admin" }, RegistrationDate = "2025-01-01" },
                new UserInfo { UserId = 2, Email = "realtor@example.com", Phone = "+79001234568",
                    IsActive = true, Roles = new List<string> { "landlord" }, RegistrationDate = "2025-01-02" },
                new UserInfo { UserId = 3, Email = "client@example.com", Phone = "+79001234569",
                    IsActive = true, Roles = new List<string> { "tenant" }, RegistrationDate = "2025-01-03" }
            };
        }

        private Border CreateUserRow(UserInfo user)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = (Brush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(16, 12, 16, 12)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

            // User info with initials
            var userPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            var avatar = new Border
            {
                Width = 32, Height = 32, CornerRadius = new CornerRadius(16),
                Background = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
                Margin = new Thickness(0, 0, 10, 0),
                Child = new TextBlock
                {
                    Text = user.Email.Substring(0, 2).ToUpper(),
                    Foreground = new SolidColorBrush(Color.FromRgb(0x60, 0x60, 0x60)),
                    FontSize = 12, FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            userPanel.Children.Add(avatar);
            userPanel.Children.Add(new TextBlock
            {
                Text = user.Email, FontSize = 13, FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center
            });
            Grid.SetColumn(userPanel, 0);
            grid.Children.Add(userPanel);

            AddCell(grid, user.RolesText, 1);
            AddCell(grid, user.Phone ?? "-", 2);

            var statusBorder = new Border
            {
                CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 2, 8, 2),
                Background = user.IsActive
                    ? new SolidColorBrush(Color.FromRgb(0xDC, 0xFC, 0xE7))
                    : new SolidColorBrush(Color.FromRgb(0xFE, 0xF2, 0xF2)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = user.StatusText, FontSize = 12,
                    Foreground = user.IsActive ? (Brush)FindResource("SuccessBrush") : (Brush)FindResource("DangerBrush")
                }
            };
            Grid.SetColumn(statusBorder, 3);
            grid.Children.Add(statusBorder);

            border.Child = grid;
            return border;
        }

        // ===== Settings Tab =====
        private void ShowSettings()
        {
            HighlightNav("settings");
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Настройки сохранены", true);
        }

        // ===== Helpers =====
        private Border CreateTableRow(string[] cells, bool isHeader)
        {
            var border = new Border
            {
                Background = isHeader
                    ? new SolidColorBrush(Color.FromRgb(0xF8, 0xF8, 0xF8)) : Brushes.White,
                BorderBrush = (Brush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(16, 10, 16, 10)
            };

            var grid = new Grid();
            for (int i = 0; i < cells.Length; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = i == 0 && cells.Length > 5 ? new GridLength(50) : new GridLength(1, GridUnitType.Star)
                });
                if (!string.IsNullOrEmpty(cells[i]))
                {
                    var tb = new TextBlock
                    {
                        Text = cells[i], FontSize = 13,
                        FontWeight = isHeader ? FontWeights.SemiBold : FontWeights.Normal,
                        Foreground = (Brush)FindResource("TextSecondaryBrush"),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(tb, i);
                    grid.Children.Add(tb);
                }
            }

            border.Child = grid;
            return border;
        }

        private void AddCell(Grid grid, string text, int column, FontWeight? weight = null)
        {
            var tb = new TextBlock
            {
                Text = text, FontSize = 13,
                FontWeight = weight ?? FontWeights.Normal,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            Grid.SetColumn(tb, column);
            grid.Children.Add(tb);
        }

        private void ApproveProperty_Click(object sender, RoutedEventArgs e)
        {
            var propertyId = (int)((Button)sender).Tag;
            try
            {
                ApiService.UpdatePropertyStatus(propertyId, 2);
                ShowMessage("Объявление одобрено", true);
                LoadAllData();
                RenderPropertiesTable();
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка: " + ex.Message, false);
            }
        }

        private void RejectProperty_Click(object sender, RoutedEventArgs e)
        {
            var propertyId = (int)((Button)sender).Tag;
            try
            {
                ApiService.UpdatePropertyStatus(propertyId, 3);
                ShowMessage("Объявление отклонено", true);
                LoadAllData();
                RenderPropertiesTable();
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка: " + ex.Message, false);
            }
        }

        private void ShowMessage(string text, bool isSuccess)
        {
            MessageText.Text = text;
            MessageText.Foreground = Brushes.White;
            MessageBorder.Background = isSuccess
                ? (Brush)FindResource("SuccessBrush") : (Brush)FindResource("DangerBrush");
            MessageBorder.Visibility = Visibility.Visible;
        }

        // ===== Navigation Events =====
        private void NavProperties_Click(object sender, RoutedEventArgs e) => ShowProperties();
        private void NavUsers_Click(object sender, RoutedEventArgs e) => ShowUsers();
        private void NavSettings_Click(object sender, RoutedEventArgs e) => ShowSettings();

        private void PropsSearch_TextChanged(object sender, TextChangedEventArgs e) => RenderPropertiesTable();
        private void UsersSearch_TextChanged(object sender, TextChangedEventArgs e) => RenderUsersTable();

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            var catalog = new MainCatalogWindow();
            catalog.Show();
            Close();
        }
    }
}
