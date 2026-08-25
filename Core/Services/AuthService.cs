using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BoostX.Core.Services
{
    public class UserAccount
    {
        public string BoostXId { get; set; } = "";
        public string Username { get; set; } = "";
        public bool IsLoggedIn { get; set; } = false;
    }

    public static class AuthService
    {
        private static readonly string SessionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BoostX", "session.json");
        public static UserAccount CurrentUser { get; private set; } = new();

        public static void InitSession()
        {
            try
            {
                if (File.Exists(SessionPath))
                {
                    var json = File.ReadAllText(SessionPath);
                    var acc = JsonSerializer.Deserialize<UserAccount>(json);
                    if (acc != null && !string.IsNullOrEmpty(acc.BoostXId))
                    {
                        CurrentUser = acc;
                        CurrentUser.IsLoggedIn = true;
                    }
                }
            }
            catch { }
        }

        public static bool LoginWithKey(string key)
        {
            key = key.Trim();
            if (string.IsNullOrWhiteSpace(key) || key.Length < 6) return false;

            // Локальная валидация и сохранение сессии
            CurrentUser = new UserAccount
            {
                BoostXId = key,
                Username = key.StartsWith("BX-") ? $"User_{key[3..7]}" : "BoostX_User",
                IsLoggedIn = true
            };

            SaveSession();
            return true;
        }

        public static void Logout()
        {
            CurrentUser = new UserAccount();
            if (File.Exists(SessionPath))
            {
                try { File.Delete(SessionPath); } catch { }
            }
        }

        private static void SaveSession()
        {
            try
            {
                var dir = Path.GetDirectoryName(SessionPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(CurrentUser);
                File.WriteAllText(SessionPath, json);
            }
            catch { }
        }
    }
}