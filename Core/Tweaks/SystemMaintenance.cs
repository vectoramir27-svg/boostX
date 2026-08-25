using System;
using System.IO;
using System.Diagnostics;
using BoostX.Core.Native;

namespace BoostX.Core.Tweaks
{
    public static class SystemMaintenance
    {
        public static void FlushRamMemory()
        {
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                try
                {
                    Win32.EmptyWorkingSet(process.Handle);
                }
                catch { }
            }
        }

        public static void CleanTempAndJunk()
        {
            string[] tempPaths = new[]
            {
                Path.GetTempPath(),
                Environment.ExpandEnvironmentVariables(@"%systemroot%\Temp"),
                Environment.ExpandEnvironmentVariables(@"%systemroot%\Prefetch")
            };

            foreach (var path in tempPaths)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        var dir = new DirectoryInfo(path);
                        foreach (var file in dir.GetFiles())
                        {
                            try { file.Delete(); } catch { }
                        }
                        foreach (var sub in dir.GetDirectories())
                        {
                            try { sub.Delete(true); } catch { }
                        }
                    }
                }
                catch { }
            }
        }

        public static void CompressSystemFiles(bool compress)
        {
            string flag = compress ? "/c" : "/u";
            TokenElevation.ExecuteCommand($"compact {flag} /s:\"C:\\Windows\" /i /q");
        }

        public static void RunScriptWithTrustedInstaller(string filePath)
        {
            if (File.Exists(filePath))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext == ".ps1")
                {
                    TokenElevation.ExecutePowerShell(File.ReadAllText(filePath));
                }
                else if (ext == ".bat" || ext == ".cmd" || ext == ".reg")
                {
                    TokenElevation.ExecuteCommand($"\"{filePath}\"");
                }
            }
        }
    }
}