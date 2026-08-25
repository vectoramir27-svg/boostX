using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace BoostX.Core.Services
{
    public class UpdateInfo
    {
        public string LatestVersion { get; set; } = "1.0.2";
        public string DownloadUrl { get; set; } = "";
        public string Changelog { get; set; } = "";
    }

    public static class UpdateService
    {
        // Текущая версия программы на ПК
        public const string CurrentVersion = "1.0.2";

        private const string UpdateCheckUrl = "https://raw.githubusercontent.com/vectoramir27-svg/boostX/main/version.json";

        public static async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "BoostX-App");

                string url = $"{UpdateCheckUrl}?r={Guid.NewGuid()}";
                string json = await client.GetStringAsync(url);

                // Отладочное окно: покажет ровно то, что скачалось с GitHub
                MessageBox.Show($"Сырой ответ от GitHub:\n\n{json}", "Диагностика апдейтера", MessageBoxButton.OK, MessageBoxImage.Information);

                // Ручной парсинг без капризных библиотек
                var info = new UpdateInfo();
                
                if (json.Contains("1.0.3"))
                    info.LatestVersion = "1.0.3";
                else if (json.Contains("1.0.2"))
                    info.LatestVersion = "1.0.2";
                else
                    info.LatestVersion = "1.0.3"; // Принудительно для теста

                info.DownloadUrl = "https://github.com/vectoramir27-svg/boostX/releases/download/v1.0.3/BoostX.exe";
                info.Changelog = "Тестовое обновление v1.0.3";

                return info;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сети при проверке:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static async Task<bool> DownloadAndInstallUpdateAsync(string downloadUrl)
        {
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "BoostX_Update.exe");
                var currentExe = Process.GetCurrentProcess().MainModule?.FileName;
                if (string.IsNullOrEmpty(currentExe)) return false;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "BoostX-App");
                    var data = await client.GetByteArrayAsync(downloadUrl);
                    await File.WriteAllBytesAsync(tempPath, data);
                }

                var updaterBat = Path.Combine(Path.GetTempPath(), "boostx_updater.bat");
                var batContent = $@"
@echo off
timeout /t 1 /nobreak > nul
move /y ""{tempPath}"" ""{currentExe}""
start """" ""{currentExe}""
del ""%~f0""
";
                await File.WriteAllTextAsync(updaterBat, batContent);

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterBat,
                    CreateNoWindow = true,
                    UseShellExecute = false
                });

                Process.GetCurrentProcess().Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
