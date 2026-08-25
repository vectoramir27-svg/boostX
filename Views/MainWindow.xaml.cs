using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BoostX.Core.Native;
using BoostX.Core.Services;
using BoostX.Core.Tweaks;
using BoostX.ViewModels;
using Microsoft.Win32;

namespace BoostX.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel? ViewModel => DataContext as MainViewModel;
        private int _toastToken = 0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var handle = new WindowInteropHelper(this).Handle;
                Win32.ApplyModernWindowStyle(handle);

                AuthService.InitSession();
                UpdateAccountUI();

                if (ViewModel != null)
                {
                    ViewModel.OnTweakExecuted += (title, isEnabled) =>
                    {
                        string state = isEnabled ? "включено" : "отключено";
                        ShowToast($"{title}: {state}", isEnabled);
                    };
                }
            };
        }

        private void UpdateAccountUI()
        {
            if (AuthService.CurrentUser.IsLoggedIn)
            {
                TxtAccountStatus.Text = $"BoostX ID: {AuthService.CurrentUser.BoostXId} (Синхронизация активна)";
                TxtAccountStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xC7, 0x59));
                TxtBoostXKey.Text = AuthService.CurrentUser.BoostXId;
            }
            else
            {
                TxtAccountStatus.Text = "Сессия: Гостевой режим (Локально)";
                TxtAccountStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));
            }
        }

        private void BtnLoginKey_Click(object sender, RoutedEventArgs e)
        {
            var key = TxtBoostXKey.Text;
            if (AuthService.LoginWithKey(key))
            {
                UpdateAccountUI();
                ShowToast("Вход по BoostX ID выполнен!", true);
            }
            else
            {
                ShowToast("Неверный формат ключа BoostX ID", false);
            }
        }

        private void BtnGetBotKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ссылка на авторизационного бота проекта
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://t.me/wonderfultech",
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private void TelegramLink_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://t.me/wonderfultech",
                    UseShellExecute = true
                });
            }
            catch { }
        }

        // Исправленная смена цвета: гарантированно переопределяет словарь ресурсов и перерисовывает окно
        private void ColorTheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string hex)
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                var brush = new SolidColorBrush(color);
                brush.Freeze();

                Application.Current.Resources["AccentColorBrush"] = brush;
                this.Resources["AccentColorBrush"] = brush;

                ShowToast("Тема оформления обновлена!", true);
            }
        }

        public void ShowToast(string message, bool isSuccess = true)
        {
            Dispatcher.Invoke(async () =>
            {
                int currentToken = ++_toastToken;

                ToastMessageText.Text = message;
                ToastIconBg.Background = (Brush)FindResource("AccentColorBrush");
                ToastIconPath.Data = Geometry.Parse(isSuccess ? "M4,11 L9,16 L18,7" : "M6,6 L18,18 M18,6 L6,18");

                var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(250))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                var slideUp = new DoubleAnimation(25.0, 0.0, TimeSpan.FromMilliseconds(250))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                ToastNotification.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                ToastTranslateTransform.BeginAnimation(TranslateTransform.YProperty, slideUp);

                await Task.Delay(2500);

                if (currentToken == _toastToken)
                {
                    var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(250))
                    {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                    };
                    var slideDown = new DoubleAnimation(0.0, 20.0, TimeSpan.FromMilliseconds(250))
                    {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                    };

                    ToastNotification.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                    ToastTranslateTransform.BeginAnimation(TranslateTransform.YProperty, slideDown);
                }
            });
        }

        private void TriggerPageAnimation()
        {
            if (PageContentContainer == null || PageTranslateTransform == null) return;

            var fadeAnim = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(280))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var slideAnim = new DoubleAnimation(15.0, 0.0, TimeSpan.FromMilliseconds(280))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            PageContentContainer.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            PageTranslateTransform.BeginAnimation(TranslateTransform.YProperty, slideAnim);
        }

        private void SidebarBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double opacity = Math.Clamp((width - 100.0) / 100.0, 0.0, 1.0);

            if (LogoTextContainer != null) LogoTextContainer.Opacity = opacity;
            if (AdminBadge != null) AdminBadge.Opacity = opacity;
            if (TxtNav1 != null) TxtNav1.Opacity = opacity;
            if (TxtNav2 != null) TxtNav2.Opacity = opacity;
            if (TxtNav3 != null) TxtNav3.Opacity = opacity;
            if (TxtNav4 != null) TxtNav4.Opacity = opacity;
            if (TxtNav5 != null) TxtNav5.Opacity = opacity;
            if (TxtNav6 != null) TxtNav6.Opacity = opacity;

            var visibility = opacity <= 0.05 ? Visibility.Collapsed : Visibility.Visible;
            if (AdminBadge != null) AdminBadge.Visibility = visibility;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnChangelog_Click(object sender, RoutedEventArgs e)
        {
            ChangelogModal.Visibility = Visibility.Visible;
            var anim = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(200));
            ChangelogModal.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void BtnCloseChangelog_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(180));
            anim.Completed += (s, ev) => ChangelogModal.Visibility = Visibility.Collapsed;
            ChangelogModal.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsModal.Visibility = Visibility.Visible;
            var anim = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(200));
            SettingsModal.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void BtnCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(180));
            anim.Completed += (s, ev) => SettingsModal.Visibility = Visibility.Collapsed;
            SettingsModal.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void BtnExportPreset_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            var sfd = new SaveFileDialog
            {
                Filter = "BoostX Preset (*.boostx)|*.boostx",
                FileName = "MyOptimization.boostx"
            };

            if (sfd.ShowDialog() == true)
            {
                var allTweaks = ViewModel.PrivacyTweaksList
                    .Concat(ViewModel.PerformanceTweaksList)
                    .Concat(ViewModel.SecurityTweaksList)
                    .Concat(ViewModel.ServicesTweaksList);

                PresetService.SaveToFile(sfd.FileName, allTweaks, "Custom");
                ShowToast("Пресет успешно сохранен в файл!", true);
            }
        }

        private void BtnImportPreset_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            var ofd = new OpenFileDialog
            {
                Filter = "BoostX Preset (*.boostx)|*.boostx"
            };

            if (ofd.ShowDialog() == true)
            {
                var preset = PresetService.LoadFromFile(ofd.FileName);
                if (preset != null)
                {
                    var allTweaks = ViewModel.PrivacyTweaksList
                        .Concat(ViewModel.PerformanceTweaksList)
                        .Concat(ViewModel.SecurityTweaksList)
                        .Concat(ViewModel.ServicesTweaksList);

                    PresetService.ApplyPreset(preset, allTweaks);
                    ShowToast("Пресет применен: все настройки активированы!", true);
                }
            }
        }

        private async void BtnCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            ShowToast("Проверка сервера обновлений...", true);
            var update = await UpdateService.CheckForUpdatesAsync();

            if (update != null && update.LatestVersion != UpdateService.CurrentVersion)
            {
                var res = MessageBox.Show($"Доступна новая версия v{update.LatestVersion}!\n\nЧто нового:\n{update.Changelog}\n\nОбновить прямо сейчас?", "Обновление boostX", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (res == MessageBoxResult.Yes)
                {
                    ShowToast("Загрузка обновления...", true);
                    await UpdateService.DownloadAndInstallUpdateAsync(update.DownloadUrl);
                }
            }
            else
            {
                ShowToast("У вас установлена самая свежая версия v1.0.0", true);
            }
        }

        private void NavTab_Checked(object sender, RoutedEventArgs e)
        {
            if (TweaksScrollViewer == null || AppsPanel == null || CleanerPanel == null || ViewModel == null) return;

            TweaksScrollViewer.Visibility = Visibility.Visible;
            AppsPanel.Visibility = Visibility.Collapsed;
            CleanerPanel.Visibility = Visibility.Collapsed;

            if (NavPrivacy.IsChecked == true)
                TweaksItemsControl.ItemsSource = ViewModel.PrivacyTweaksList;
            else if (NavPerformance.IsChecked == true)
                TweaksItemsControl.ItemsSource = ViewModel.PerformanceTweaksList;
            else if (NavSecurity.IsChecked == true)
                TweaksItemsControl.ItemsSource = ViewModel.SecurityTweaksList;
            else if (NavServices.IsChecked == true)
                TweaksItemsControl.ItemsSource = ViewModel.ServicesTweaksList;
            else if (NavApps.IsChecked == true)
            {
                TweaksScrollViewer.Visibility = Visibility.Collapsed;
                AppsPanel.Visibility = Visibility.Visible;
            }
            else if (NavCleaner.IsChecked == true)
            {
                TweaksScrollViewer.Visibility = Visibility.Collapsed;
                CleanerPanel.Visibility = Visibility.Visible;
            }

            TriggerPageAnimation();
        }

        private void BtnRemoveOneDrive_Click(object sender, RoutedEventArgs e)
        {
            BloatwareTweaks.UninstallOneDrive();
            ShowToast("Microsoft OneDrive успешно удален!", true);
        }

        private void BtnRemoveUwp_Click(object sender, RoutedEventArgs e)
        {
            BloatwareTweaks.RemoveStandardUwpApps();
            ShowToast("Встроенные UWP-приложения очищены!", true);
        }

        private void BtnDisableAi_Click(object sender, RoutedEventArgs e)
        {
            BloatwareTweaks.DisableWindowsAI(true);
            ShowToast("Copilot, Recall и Cortana отключены!", true);
        }

        private async void BtnFlushRam_Click(object sender, RoutedEventArgs e)
        {
            SystemMaintenance.FlushRamMemory();
            if (ViewModel != null) await ViewModel.LoadSystemStatsAsync();
            ShowToast("Оперативная память очищена!", true);
        }

        private async void BtnCleanTemp_Click(object sender, RoutedEventArgs e)
        {
            SystemMaintenance.CleanTempAndJunk();
            if (ViewModel != null) await ViewModel.LoadSystemStatsAsync();
            ShowToast("Временные файлы Temp и Prefetch удалены!", true);
        }

        private async void BtnCleanUpdates_Click(object sender, RoutedEventArgs e)
        {
            ServicesTweaks.ClearUpdateCache();
            if (ViewModel != null) await ViewModel.LoadSystemStatsAsync();
            ShowToast("Кэш обновлений SoftwareDistribution очищен!", true);
        }
    }
}