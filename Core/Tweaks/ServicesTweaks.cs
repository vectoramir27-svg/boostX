using Microsoft.Win32;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class ServicesTweaks
    {
        private static readonly string[] UnnecessaryServices = new[]
        {
            "DiagTrack",          // Служба диагностического отслеживания
            "dmwappushservice",   // WAP Push маршрутизация
            "MapsBroker",         // Загруженные карты
            "SensorDataService",  // Служба данных датчиков
            "SensorService",      // Служба датчиков
            "SensrSvc",           // Служба наблюдения за датчиками
            "Fax",                // Факс
            "RetailDemo",         // Демонстрационный режим
            "WerSvc",             // Отчеты об ошибках Windows
            "PcaSvc"              // Помощник по совместимости программ
        };

        public static void SetUnnecessaryServices(bool disable)
        {
            string startMode = disable ? "disabled" : "demand";
            foreach (var svc in UnnecessaryServices)
            {
                TokenElevation.ExecuteCommand($"sc config \"{svc}\" start= {startMode}");
                if (disable)
                {
                    TokenElevation.ExecuteCommand($"sc stop \"{svc}\"");
                }
            }
        }

        public static void SetWindowsUpdates(bool disable)
        {
            if (disable)
            {
                TokenElevation.ExecuteCommand("sc stop wuauserv");
                TokenElevation.ExecuteCommand("sc config wuauserv start= disabled");
                TokenElevation.ExecuteCommand("sc stop bits");
                TokenElevation.ExecuteCommand("sc config bits start= disabled");
                TokenElevation.ExecuteCommand("sc stop dosvc");
                TokenElevation.ExecuteCommand("sc config dosvc start= disabled");

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", 1, RegistryValueKind.DWord);
            }
            else
            {
                TokenElevation.ExecuteCommand("sc config wuauserv start= auto");
                TokenElevation.ExecuteCommand("sc config bits start= auto");
                TokenElevation.ExecuteCommand("sc config dosvc start= auto");

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", 0, RegistryValueKind.DWord);
            }
        }

        public static void ClearUpdateCache()
        {
            TokenElevation.ExecuteCommand("sc stop wuauserv");
            TokenElevation.ExecuteCommand(@"rd /s /q C:\Windows\SoftwareDistribution\Download");
            TokenElevation.ExecuteCommand(@"md C:\Windows\SoftwareDistribution\Download");
            TokenElevation.ExecuteCommand("sc start wuauserv");
        }
    }
}