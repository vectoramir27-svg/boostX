using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class BloatwareTweaks
    {
        public static void DisableWindowsAI(bool disable)
        {
            int val = disable ? 1 : 0;

            // Windows Copilot
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\WindowsCopilot", "TurnOffWindowsCopilot", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot", "TurnOffWindowsCopilot", val, RegistryValueKind.DWord);

            // Windows Recall / AI Snapshots
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsAI", "DisableAIDataAnalysis", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\WindowsAI", "DisableAIDataAnalysis", val, RegistryValueKind.DWord);

            // Cortana
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", disable ? 0 : 1, RegistryValueKind.DWord);
        }

        public static void UninstallOneDrive()
        {
            TokenElevation.ExecuteCommand("taskkill /f /im OneDrive.exe");

            string sysX64 = @"C:\Windows\SysWOW64\OneDriveSetup.exe";
            string sysX86 = @"C:\Windows\System32\OneDriveSetup.exe";
            string target = File.Exists(sysX64) ? sysX64 : sysX86;

            if (File.Exists(target))
            {
                TokenElevation.ExecuteCommand($"\"{target}\" /uninstall");
            }

            // Запрет повторной установки через GPO
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableFileSyncNGSC", 1, RegistryValueKind.DWord);
        }

        public static void RemoveMicrosoftEdge()
        {
            // Остановка фоновых процессов
            TokenElevation.ExecuteCommand("taskkill /f /im msedge.exe");
            TokenElevation.ExecuteCommand("taskkill /f /im msedgewebview2.exe");

            // Отключение автообновления и автозапуска Edge
            TokenElevation.ExecuteCommand("sc stop MicrosoftEdgeElevationService");
            TokenElevation.ExecuteCommand("sc config MicrosoftEdgeElevationService start= disabled");
            TokenElevation.ExecuteCommand("sc stop edgeupdate");
            TokenElevation.ExecuteCommand("sc config edgeupdate start= disabled");
            TokenElevation.ExecuteCommand("sc stop edgeupdatem");
            TokenElevation.ExecuteCommand("sc config edgeupdatem start= disabled");

            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\EdgeUpdate", "DoNotUpdateToEdgeWithChromium", 1, RegistryValueKind.DWord);
        }

        public static void RemoveStandardUwpApps()
        {
            string[] apps = new[]
            {
                "*Microsoft.3DBuilder*",
                "*Microsoft.BingWeather*",
                "*Microsoft.GetHelp*",
                "*Microsoft.Getstarted*",
                "*Microsoft.MicrosoftSolitaireCollection*",
                "*Microsoft.People*",
                "*Microsoft.WindowsMaps*",
                "*Microsoft.YourPhone*",
                "*Microsoft.ZuneMusic*",
                "*Microsoft.ZuneVideo*",
                "*Microsoft.SkypeApp*",
                "*Microsoft.WindowsFeedbackHub*"
            };

            foreach (var app in apps)
            {
                string script = $"Get-AppxPackage -AllUsers '{app}' | Remove-AppxPackage -AllUsers; " +
                               $"Get-AppxProvisionedPackage -Online | Where-Object DisplayName -like '{app}' | Remove-AppxProvisionedPackage -Online";
                TokenElevation.ExecutePowerShell(script);
            }
        }
    }
}