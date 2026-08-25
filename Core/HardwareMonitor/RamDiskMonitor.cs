using System;
using System.IO;
using System.Management;

namespace BoostX.Core.HardwareMonitor
{
    public static class RamDiskMonitor
    {
        public static (double usedGb, double totalGb) GetRamUsage()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    double totalKb = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                    double freeKb = Convert.ToDouble(obj["FreePhysicalMemory"]);
                    double totalGb = Math.Round(totalKb / 1024 / 1024, 1);
                    double usedGb = Math.Round((totalKb - freeKb) / 1024 / 1024, 1);
                    return (usedGb, totalGb);
                }
            }
            catch { }
            return (0, 0);
        }

        public static (double freeGb, double totalGb) GetSystemDiskSpace()
        {
            try
            {
                var drive = new DriveInfo("C");
                if (drive.IsReady)
                {
                    double freeGb = Math.Round((double)drive.AvailableFreeSpace / 1024 / 1024 / 1024, 1);
                    double totalGb = Math.Round((double)drive.TotalSize / 1024 / 1024 / 1024, 1);
                    return (freeGb, totalGb);
                }
            }
            catch { }
            return (0, 0);
        }
    }
}