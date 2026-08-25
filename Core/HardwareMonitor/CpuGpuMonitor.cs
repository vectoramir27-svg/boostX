using System;
using System.Management;

namespace BoostX.Core.HardwareMonitor
{
    public static class CpuGpuMonitor
    {
        public static string GetCpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    return obj["Name"]?.ToString()?.Trim() ?? "Unknown CPU";
                }
            }
            catch { }
            return "AMD / Intel CPU";
        }

        public static string GetGpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                foreach (var obj in searcher.Get())
                {
                    return obj["Name"]?.ToString()?.Trim() ?? "Unknown GPU";
                }
            }
            catch { }
            return "Graphics Card";
        }
    }
}