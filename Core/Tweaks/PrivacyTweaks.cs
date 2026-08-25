using System;
using System.IO;
using Microsoft.Win32;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class PrivacyTweaks
    {
        public static void SetWindowsTelemetry(bool disable)
        {
            int val = disable ? 0 : 3;

            // AllowTelemetry GPO
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", val, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackNotifications", disable ? 1 : 0, RegistryValueKind.DWord);

            // Advertising ID
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", disable ? 0 : 1, RegistryValueKind.DWord);

            // Запрет сбора рукописного и клавиатурного ввода (Keylogger)
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization", "RestrictImplicitInkCollection", disable ? 1 : 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", disable ? 1 : 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization\TrainedDataStore", "HarvestContacts", disable ? 0 : 1, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Personalization\Settings", "AcceptedPrivacyPolicy", disable ? 0 : 1, RegistryValueKind.DWord);
        }

        public static void SetGpuTelemetry(bool disable)
        {
            string state = disable ? "/Disable" : "/Enable";
            string[] tasks = new[]
            {
                @"\NvTmMon_{*",
                @"\NvTmRep_{*",
                @"\NvTmRepOnLogon_{*",
                @"\Intel\Intel PTT EK Recertification",
                @"\Intel\SUR\QCUST"
            };

            foreach (var task in tasks)
            {
                TokenElevation.ExecuteCommand($"schtasks /Change /TN \"{task}\" {state}");
            }
        }

        public static void BlockTelemetryHosts(bool block)
        {
            string hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
            string marker = "# [BoostX Telemetry Block]";
            
            string[] blockedDomains = new[]
            {
                "0.0.0.0 v10.events.data.microsoft.com",
                "0.0.0.0 telemetry.microsoft.com",
                "0.0.0.0 vortex.data.microsoft.com",
                "0.0.0.0 watson.telemetry.microsoft.com",
                "0.0.0.0 settings-win.data.microsoft.com",
                "0.0.0.0 diagnostic.support.microsoft.com",
                "0.0.0.0 feedback.windows.com",
                "0.0.0.0 vortex-win.data.microsoft.com"
            };

            try
            {
                if (File.Exists(hostsPath))
                {
                    string content = File.ReadAllText(hostsPath);
                    if (block && !content.Contains(marker))
                    {
                        using var sw = File.AppendText(hostsPath);
                        sw.WriteLine(Environment.NewLine + marker);
                        foreach (var d in blockedDomains) sw.WriteLine(d);
                    }
                    else if (!block && content.Contains(marker))
                    {
                        var lines = File.ReadAllLines(hostsPath);
                        var filtered = Array.FindAll(lines, l => !l.Contains(marker) && !Array.Exists(blockedDomains, d => d == l.Trim()));
                        File.WriteAllLines(hostsPath, filtered);
                    }
                }
            }
            catch { }
        }
    }
}