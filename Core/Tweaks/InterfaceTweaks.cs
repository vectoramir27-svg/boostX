using Microsoft.Win32;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class InterfaceTweaks
    {
        public static void SetClassicContextMenuWin11(bool enableClassic)
        {
            if (enableClassic)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "", RegistryValueKind.String);
            }
            else
            {
                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", false);
                }
                catch { }
            }
            RestartExplorer();
        }

        public static void DisableSystemAdsAndTips(bool disable)
        {
            int val = disable ? 0 : 1;

            // Отключение предложений в меню Пуск и Проводнике
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338388Enabled", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338389Enabled", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-353696Enabled", val, RegistryValueKind.DWord);

            // Отключение окна первоначальной настройки Windows (SCOOBE)
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement", "ScoobeSystemSettingEnabled", val, RegistryValueKind.DWord);

            // Отключение советов Windows
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SoftLandingEnabled", val, RegistryValueKind.DWord);
        }

        public static void RestartExplorer()
        {
            TokenElevation.ExecuteCommand("taskkill /f /im explorer.exe & start explorer.exe");
        }
    }
}