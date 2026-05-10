using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ArendaDesktop.Helpers;
using ArendaDesktop.Models;
using ArendaDesktop.Services;

namespace ArendaDesktop.Windows
{
    public partial class AdminWindow : Window
    {
        private string _activeTab = "users";
        private List<Property> _allProperties = new List<Property>();

        public AdminWindow()
        {
            InitializeComponent();
            SetupProfile();
            LoadAllData();
            ShowUsers();
        }

        private void SetupProfile()
        {
            AdminNameText.Text = AuthHelper.Email ?? "Администратор";
            AvatarText.Text = !string.IsNullOrEmpty(AuthHelper.Email)
                ? AuthHelper.Email.Substring(0, 1).ToUpper()
                : "A";
        }

        private void LoadAllData()
        {
            try
            {
                var result = ApiService.GetAllPropertiesForAdmin(1, 100);
                _allProperties = result.Items;
                PropertiesCountText.Text = result.Total.ToString();
                PendingCountText.Text = _allProperties.Count(p => p.StatusId == 1).ToString();
                UsersCountText.Text = "3";
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
            NavUsers.Background = tab == "users"
                ? new SolidColorBrush(Color.FromRgb(0xEF, 0xF6, 0xFF))
                : Brushes.White;
            NavProperties.Background = tab == "properties"
                ? new SolidColorBrush(Color.FromRgb(0xEF, 0xF6, 0xFF))
                : Brushes.White;
            NavModeration.Background = tab == "moderation"
                ? new SolidColorBrush(Color.FromRgb(0xEF, 0xF6, 0xFF))
                : Brushes.White;
        }

        // ===== Users Tab =====
        private void ShowUsers()
        {
            HighlightNav("users");
            PageTitle.Text = "Управление пользователями";
            PageSubtitle.Text = "Управление зарегистрированными пользователями";
            ContentPanel.Children.Clear();

            var users = GetDemoUsers();
            var search = SearchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Email.ToLower().Contains(search)).ToList();
            }

            // Table header
            var header = CreateTableRow(
                new[] { "Email", "Телефон", "Роли", "Дата рег.", "Статус", "Действия" },
                true
            );
            ContentPanel.Children.Add(header);

            foreach (var user in users)
            {
                var row = CreateUserRow(user);
                ContentPanel.Children.Add(row);
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
            for (int i = 0; i < 6; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = i == 5 ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
                });
            }

            AddCell(grid, user.Email, 0, FontWeights.SemiBold);
            AddCell(grid, user.Phone ?? "-", 1);
            AddCell(grid, user.RolesText, 2);

            DateTime.TryParse(user.RegistrationDate, out DateTime regDate);
            AddCell(grid, regDate.ToString("dd.MM.yyyy"), 3);

            // Status badge
            var statusBorder = new Border
            {
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 2, 8, 2),
                Background = user.IsActive
                    ? new SolidColorBrush(Color.FromRgb(0xDC, 0xFC, 0xE7))
                    : new SolidColorBrush(Color.FromRgb(0xFE, 0xF2, 0xF2)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = user.StatusText,
                    FontSize = 12,
                    Foreground = user.IsActive
                        ? (Brush)FindResource("SuccessBrush")
                        : (Brush)FindResource("DangerBrush")
                }
            };
            Grid.SetColumn(statusBorder, 4);
            grid.Children.Add(statusBorder);

            // Action button
            var actionBtn = new Button
            {
                Content = user.IsActive ? "Заблокировать" : "Разблокировать",
                Style = user.IsActive
                    ? (Style)FindResource("DangerButton")
                    : (Style)FindResource("SuccessButton"),
                Padding = new Thickness(10, 4, 10, 4),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = user
            };
            actionBtn.Click += ToggleUserStatus_Click;
            Grid.SetColumn(actionBtn, 5);
            grid.Children.Add(actionBtn);

            border.Child = grid;
            return border;
        }

        private void ToggleUserStatus_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Статус пользователя изменён", true);
            ShowUsers();
        }

        // ===== Properties Tab =====
        private void ShowProperties()
        {
            HighlightNav("properties");
            PageTitle.Text = "Управление объявлениями";
            PageSubtitle.Text = "Все объявления на платформе";
            ContentPanel.Children.Clear();

            var search = SearchBox.Text.Trim().ToLower();
            var filtered = _allProperties.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(p =>
                    p.Title.ToLower().Contains(search) ||
                    p.City.ToLower().Contains(search));
            }

            // Table header
            var header = CreateTableRow(
                new[] { "ID", "Название", "Город", "Цена", "Статус", "Действия" },
                true
            );
            ContentPanel.Children.Add(header);

            foreach (var prop in filtered)
            {
                ContentPanel.Children.Add(CreatePropertyRow(prop));
            }
        }

        private Border CreatePropertyRow(Property prop)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = (Brush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(16, 12, 16, 12)
            };

            var grid = new Grid();
            for (int i = 0; i < 6; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = i == 5 ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
                });
            }

            AddCell(grid, prop.PropertyId.ToString(), 0);
            AddCell(grid, prop.Title, 1, FontWeights.SemiBold);
            AddCell(grid, prop.City, 2);
            AddCell(grid, prop.PriceFormatted, 3);

            // Status
            Brush statusBg;
            Brush statusFg;
            switch (prop.StatusId)
            {
                case 2:
                    statusBg = new SolidColorBrush(Color.FromRgb(0xDC, 0xFC, 0xE7));
                    statusFg = (Brush)FindResource("SuccessBrush");
                    break;
                case 3:
                    statusBg = new SolidColorBrush(Color.FromRgb(0xFE, 0xF2, 0xF2));
                    statusFg = (Brush)FindResource("DangerBrush");
                    break;
                default:
                    statusBg = new SolidColorBrush(Color.FromRgb(0xFE, 0xF9, 0xC3));
                    statusFg = (Brush)FindResource("WarningBrush");
                    break;
            }
            var statusBorder = new Border
            {
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 2, 8, 2),
                Background = statusBg,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock { Text = prop.StatusText, FontSize = 12, Foreground = statusFg }
            };
            Grid.SetColumn(statusBorder, 4);
            grid.Children.Add(statusBorder);

            // Actions
            var actions = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            var viewBtn = new Button
            {
                Content = "👁",
                Style = (Style)FindResource("SecondaryButton"),
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 14,
                Margin = new Thickness(0, 0, 4, 0),
                Tag = prop.PropertyId
            };
            viewBtn.Click += (s, e) =>
            {
                var details = new PropertyDetailsWindow((int)((Button)s).Tag);
                details.ShowDialog();
            };
            actions.Children.Add(viewBtn);

            if (prop.StatusId == 1)
            {
                var approveBtn = new Button
                {
                    Content = "✓",
                    Style = (Style)FindResource("SuccessButton"),
                    Padding = new Thickness(8, 4, 8, 4),
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 4, 0),
                    Tag = prop.PropertyId
                };
                approveBtn.Click += ApproveProperty_Click;
                actions.Children.Add(approveBtn);

                var rejectBtn = new Button
                {
                    Content = "✗",
                    Style = (Style)FindResource("DangerButton"),
                    Padding = new Thickness(8, 4, 8, 4),
                    FontSize = 14,
                    Tag = prop.PropertyId
                };
                rejectBtn.Click += RejectProperty_Click;
                actions.Children.Add(rejectBtn);
            }
            Grid.SetColumn(actions, 5);
            grid.Children.Add(actions);

            border.Child = grid;
            return border;
        }

        // ===== Moderation Tab =====
        private void ShowModeration()
        {
            HighlightNav("moderation");
            PageTitle.Text = "Модерация объявлений";
            PageSubtitle.Text = "Объявления, ожидающие проверки";
            ContentPanel.Children.Clear();

            var pending = _allProperties.Where(p => p.StatusId == 1).ToList();

            if (pending.Count == 0)
            {
                ContentPanel.Children.Add(new TextBlock
                {
                    Text = "Нет объявлений на модерации",
                    FontSize = 16,
                    Foreground = (Brush)FindResource("TextSecondaryBrush"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 40, 0, 0)
                });
                return;
            }

            foreach (var prop in pending)
            {
                ContentPanel.Children.Add(CreateModerationCard(prop));
            }
        }

        private Border CreateModerationCard(Property prop)
        {
            var card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 12),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 6, ShadowDepth = 1, Opacity = 0.06, Color = Colors.Black
                }
            };

            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = prop.Title,
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("TextPrimaryBrush")
            });
            stack.Children.Add(new TextBlock
            {
                Text = $"📍 {prop.City}, {prop.Address}",
                FontSize = 13,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 4, 0, 0)
            });

            var details = new WrapPanel { Margin = new Thickness(0, 8, 0, 0) };
            details.Children.Add(CreateInfoTag($"💰 {prop.PriceFormatted}"));
            details.Children.Add(CreateInfoTag($"📐 {prop.AreaFormatted}"));
            if (prop.Rooms.HasValue) details.Children.Add(CreateInfoTag($"🛏 {prop.Rooms} комн."));
            if (prop.Floor.HasValue) details.Children.Add(CreateInfoTag($"🏢 {prop.FloorInfo}"));
            stack.Children.Add(details);

            if (!string.IsNullOrEmpty(prop.Description))
            {
                stack.Children.Add(new TextBlock
                {
                    Text = prop.Description,
                    FontSize = 13,
                    Foreground = (Brush)FindResource("TextSecondaryBrush"),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 8, 0, 0),
                    MaxHeight = 60
                });
            }

            // Action buttons
            var actions = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 12, 0, 0)
            };
            var approveBtn = new Button
            {
                Content = "Одобрить",
                Style = (Style)FindResource("SuccessButton"),
                Padding = new Thickness(20, 8, 20, 8),
                FontSize = 13,
                Margin = new Thickness(0, 0, 8, 0),
                Tag = prop.PropertyId
            };
            approveBtn.Click += ApproveProperty_Click;
            actions.Children.Add(approveBtn);

            var rejectBtn = new Button
            {
                Content = "Отклонить",
                Style = (Style)FindResource("DangerButton"),
                Padding = new Thickness(20, 8, 20, 8),
                FontSize = 13,
                Margin = new Thickness(0, 0, 8, 0),
                Tag = prop.PropertyId
            };
            rejectBtn.Click += RejectProperty_Click;
            actions.Children.Add(rejectBtn);

            var viewBtn = new Button
            {
                Content = "Просмотр",
                Style = (Style)FindResource("SecondaryButton"),
                Padding = new Thickness(20, 8, 20, 8),
                FontSize = 13,
                Tag = prop.PropertyId
            };
            viewBtn.Click += (s, e) =>
            {
                var details2 = new PropertyDetailsWindow((int)((Button)s).Tag);
                details2.ShowDialog();
            };
            actions.Children.Add(viewBtn);

            stack.Children.Add(actions);
            card.Child = stack;
            return card;
        }

        private Border CreateInfoTag(string text)
        {
            return new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0xF1, 0xF5, 0xF9)),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(10, 4, 10, 4),
                Margin = new Thickness(0, 0, 8, 0),
                Child = new TextBlock
                {
                    Text = text,
                    FontSize = 13,
                    Foreground = (Brush)FindResource("TextPrimaryBrush")
                }
            };
        }

        // ===== Helpers =====
        private Border CreateTableRow(string[] cells, bool isHeader)
        {
            var border = new Border
            {
                Background = isHeader
                    ? new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC))
                    : Brushes.White,
                BorderBrush = (Brush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(16, 10, 16, 10)
            };

            var grid = new Grid();
            for (int i = 0; i < cells.Length; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = i == cells.Length - 1 ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
                });
                var tb = new TextBlock
                {
                    Text = cells[i],
                    FontSize = 13,
                    FontWeight = isHeader ? FontWeights.SemiBold : FontWeights.Normal,
                    Foreground = isHeader
                        ? (Brush)FindResource("TextSecondaryBrush")
                        : (Brush)FindResource("TextPrimaryBrush"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(tb, i);
                grid.Children.Add(tb);
            }

            border.Child = grid;
            return border;
        }

        private void AddCell(Grid grid, string text, int column, FontWeight? weight = null)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = 13,
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
                RefreshCurrentTab();
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
                RefreshCurrentTab();
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка: " + ex.Message, false);
            }
        }

        private void RefreshCurrentTab()
        {
            switch (_activeTab)
            {
                case "users": ShowUsers(); break;
                case "properties": ShowProperties(); break;
                case "moderation": ShowModeration(); break;
            }
        }

        private void ShowMessage(string text, bool isSuccess)
        {
            MessageText.Text = text;
            MessageBorder.Background = isSuccess
                ? (Brush)FindResource("SuccessBrush")
                : (Brush)FindResource("DangerBrush");
            MessageBorder.Visibility = Visibility.Visible;
        }

        // ===== Navigation Events =====
        private void NavUsers_Click(object sender, RoutedEventArgs e) => ShowUsers();
        private void NavProperties_Click(object sender, RoutedEventArgs e) => ShowProperties();
        private void NavModeration_Click(object sender, RoutedEventArgs e) => ShowModeration();

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) RefreshCurrentTab();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
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
