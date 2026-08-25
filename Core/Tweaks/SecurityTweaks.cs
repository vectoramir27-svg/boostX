using Microsoft.Win32;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class SecurityTweaks
    {
        public static void SetWindowsDefender(bool disable)
        {
            int val = disable ? 1 : 0;
            
            // Политики отключения Defender и Real-time Protection
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableRealtimeMonitoring", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableBehaviorMonitoring", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableOnAccessProtection", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableScanOnRealtimeEnable", val, RegistryValueKind.DWord);

            // Отключение служб через PowerShell
            if (disable)
            {
                TokenElevation.ExecutePowerShell("Set-MpPreference -DisableRealtimeMonitoring $true -DisableBehaviorMonitoring $true -DisableIOAVProtection $true");
            }
            else
            {
                TokenElevation.ExecutePowerShell("Set-MpPreference -DisableRealtimeMonitoring $false -DisableBehaviorMonitoring $false -DisableIOAVProtection $false");
            }
        }

        public static void SetSmartScreen(bool disable)
        {
            int val = disable ? 0 : 1;
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System", "EnableSmartScreen", disable ? 0 : 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AppHost", "EnableWebContentEvaluation", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "SmartScreenEnabled", disable ? "Off" : "RequireAdmin", RegistryValueKind.String);
        }

        public static void SetUac(bool disable)
        {
            // Отключение уведомлений и запросов UAC
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", disable ? 0 : 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", disable ? 0 : 5, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", disable ? 0 : 1, RegistryValueKind.DWord);
        }

        public static void SetVbs(bool disable)
        {
            // Virtualization-Based Security & Hypervisor Enforced Code Integrity (HVCI)
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard", "EnableVirtualizationBasedSecurity", disable ? 0 : 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", disable ? 0 : 1, RegistryValueKind.DWord);
        }

        public static void RunHwidActivation()
        {
            TokenElevation.ExecutePowerShell("irm https://get.activated.win | iex");
        }
    }
}