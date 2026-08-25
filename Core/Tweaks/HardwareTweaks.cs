using Microsoft.Win32;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class HardwareTweaks
    {
        public static void SetPowerScheme(bool enableUltimate)
        {
            if (enableUltimate)
            {
                TokenElevation.ExecuteCommand("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                TokenElevation.ExecuteCommand("powercfg -setactive e9a42b02-d5df-448d-aa00-03f14749eb61");
            }
            else
            {
                // Возврат на стандартную сбалансированную схему (Balanced GUID)
                TokenElevation.ExecuteCommand("powercfg -setactive 381b4222-f694-41f0-9685-ff5bb260df2e");
            }
        }

        public static void SetRealtekLatency(bool fix)
        {
            int val = fix ? 0 : 1;
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Realtek\Audio\RtkNGUI64\PowerMgnt", "Enabled", val, RegistryValueKind.DWord);
        }

        public static void DisableMouseAcceleration(bool disable)
        {
            if (disable)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseSpeed", "0", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold1", "0", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold2", "0", RegistryValueKind.String);
            }
            else
            {
                // Дефолтные значения Windows с ускорением курсора
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseSpeed", "1", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold1", "6", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold2", "10", RegistryValueKind.String);
            }
        }

        public static void SetStickyKeys(bool disable)
        {
            string flag = disable ? "506" : "510";
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\StickyKeys", "Flags", flag, RegistryValueKind.String);
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\Keyboard Response", "Flags", disable ? "122" : "126", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\ToggleKeys", "Flags", disable ? "58" : "62", RegistryValueKind.String);
        }
    }
}