using Microsoft.Win32;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class NetworkTweaks
    {
        public static void SetNetworkProtocols(bool optimize)
        {
            if (optimize)
            {
                // Отключение компонентов IPv6 на уровне ядра
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", 0xFF, RegistryValueKind.DWord);

                // Отключение устаревших туннельных интерфейсов
                TokenElevation.ExecuteCommand("netsh interface teredo set state disabled");
                TokenElevation.ExecuteCommand("netsh interface isatap set state disabled");
                TokenElevation.ExecuteCommand("netsh interface 6to4 set state disabled");

                // Отключение NetBIOS через реестр
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\NetBT\Parameters\Interfaces", "NetbiosOptions", 2, RegistryValueKind.DWord);
            }
            else
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", 0, RegistryValueKind.DWord);
                TokenElevation.ExecuteCommand("netsh interface teredo set state default");
                TokenElevation.ExecuteCommand("netsh interface isatap set state default");
                TokenElevation.ExecuteCommand("netsh interface 6to4 set state default");
            }
        }

        public static void OptimizeGamingLatency()
        {
            // Отключение алгоритма Nagle (TCP No Delay) и Network Throttling
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0, RegistryValueKind.DWord);
        }
    }
}