using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BoostX.Core.Services
{
    public class UpdateInfo
    {
        public string LatestVersion { get; set; } = "1.0.0";
        public string DownloadUrl { get; set; } = "";
        public string Changelog { get; set; } = "";
    }

    public static class UpdateService
    {
        public const string CurrentVersion = "1.0.0";
        // URL к вашему JSON-файлу манифеста на GitHub или личном сервере
        private const string UpdateCheckUrl = "https://raw.githubusercontent.com/vectoramir27-svg/boostX/main/version.json";

        public static async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var json = await client.GetStringAsync(UpdateCheckUrl);
                return JsonSerializer.Deserialize<UpdateInfo>(json);
            }
            catch
            {
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
                    var data = await client.GetByteArrayAsync(downloadUrl);
                    await File.WriteAllBytesAsync(tempPath, data);
                }

                // Скрипт бесшовного обновления: ждет завершения boostX, перезаписывает exe и запускает заново
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
