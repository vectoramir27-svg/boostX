using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BoostX.Core.Services
{
    public class UpdateInfo
    {
        [JsonPropertyName("LatestVersion")]
        public string? LatestVersionProp { get; set; }

        [JsonPropertyName("version")]
        public string? VersionProp { get; set; }

        [JsonPropertyName("DownloadUrl")]
        public string? DownloadUrlProp { get; set; }

        [JsonPropertyName("download_url")]
        public string? DownloadUrlSnake { get; set; }

        [JsonPropertyName("Changelog")]
        public string? ChangelogProp { get; set; }

        [JsonPropertyName("changelog")]
        public string? ChangelogLower { get; set; }

        public string LatestVersion => !string.IsNullOrWhiteSpace(LatestVersionProp) ? LatestVersionProp : (VersionProp ?? "");
        public string DownloadUrl => !string.IsNullOrWhiteSpace(DownloadUrlProp) ? DownloadUrlProp : (DownloadUrlSnake ?? "");
        public string Changelog => !string.IsNullOrWhiteSpace(ChangelogProp) ? ChangelogProp : (ChangelogLower ?? "Плановое обновление");
    }

    public static class UpdateService
    {
        // Твоя текущая запущенная сборка (старая)
        public const string CurrentVersion = "1.0.2";

        private const string UpdateCheckUrl = "https://raw.githubusercontent.com/vectoramir27-svg/boostX/main/version.json";

        public static async Task<(UpdateInfo? info, string rawJson, string? errorMessage)> CheckForUpdatesAsync()
        {
            try
            {
                using var handler = new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.All
                };
                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(8);

                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache, no-store, must-revalidate");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Pragma", "no-cache");

                // Уникальный параметр обхода кэша
                string noCacheUrl = $"{UpdateCheckUrl}?ts={DateTime.UtcNow.Ticks}";

                var response = await client.GetAsync(noCacheUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return (null, "", $"GitHub вернул ошибку: {(int)response.StatusCode} ({response.ReasonPhrase})");
                }

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return (null, "", "Файл version.json пустой!");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var info = JsonSerializer.Deserialize<UpdateInfo>(json, options);
                return (info, json, null);
            }
            catch (Exception ex)
            {
                return (null, "", $"Исключение сети: {ex.Message}");
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
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0");
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
