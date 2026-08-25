using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using BoostX.Core.HardwareMonitor;
using BoostX.Core.Tweaks;
using BoostX.Models;

namespace BoostX.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _cpuName = "Loading CPU...";
        private string _gpuName = "Loading GPU...";
        private string _ramUsage = "0 / 0 GB";
        private string _diskSpace = "0 / 0 GB";

        public event Action<string, bool>? OnTweakExecuted;

        public ObservableCollection<TweakItem> PrivacyTweaksList { get; } = new();
        public ObservableCollection<TweakItem> PerformanceTweaksList { get; } = new();
        public ObservableCollection<TweakItem> SecurityTweaksList { get; } = new();
        public ObservableCollection<TweakItem> ServicesTweaksList { get; } = new();

        public string CpuName
        {
            get => _cpuName;
            set { _cpuName = value; OnPropertyChanged(); }
        }

        public string GpuName
        {
            get => _gpuName;
            set { _gpuName = value; OnPropertyChanged(); }
        }

        public string RamUsage
        {
            get => _ramUsage;
            set { _ramUsage = value; OnPropertyChanged(); }
        }

        public string DiskSpace
        {
            get => _diskSpace;
            set { _diskSpace = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            InitializeTweaks();
            _ = LoadSystemStatsAsync();
        }

        private void InitializeTweaks()
        {
            // ================= 1. ПРИВАТНОСТЬ =================
            PrivacyTweaksList.Add(new TweakItem
            {
                Id = "priv_telemetry",
                Title = "Отключить телеметрию Windows",
                Description = "Блокирует службу DiagTrack, отправку дампов и рекламный идентификатор",
                OnToggle = state =>
                {
                    PrivacyTweaks.SetWindowsTelemetry(state);
                    OnTweakExecuted?.Invoke("Телеметрия Windows", state);
                }
            });

            PrivacyTweaksList.Add(new TweakItem
            {
                Id = "priv_gpu",
                Title = "Отключить телеметрию NVIDIA / Intel",
                Description = "Останавливает сбор фоновой телеметрии видеокарт в планировщике задач",
                OnToggle = state =>
                {
                    PrivacyTweaks.SetGpuTelemetry(state);
                    OnTweakExecuted?.Invoke("Телеметрия GPU", state);
                }
            });

            PrivacyTweaksList.Add(new TweakItem
            {
                Id = "priv_hosts",
                Title = "Блокировка серверов сбора через Hosts",
                Description = "Перенаправляет 40+ аналитических адресов Microsoft на 0.0.0.0",
                OnToggle = state =>
                {
                    PrivacyTweaks.BlockTelemetryHosts(state);
                    OnTweakExecuted?.Invoke("Блокировка Hosts", state);
                }
            });

            // ================= 2. ПРОИЗВОДИТЕЛЬНОСТЬ =================
            PerformanceTweaksList.Add(new TweakItem
            {
                Id = "perf_power",
                Title = "Схема «Максимальная производительность»",
                Description = "Активирует скрытый режим электропитания Ultimate Performance",
                OnToggle = state =>
                {
                    HardwareTweaks.SetPowerScheme(state);
                    OnTweakExecuted?.Invoke("План электропитания", state);
                }
            });

            PerformanceTweaksList.Add(new TweakItem
            {
                Id = "perf_realtek",
                Title = "Устранить задержку звука Realtek",
                Description = "Отключает переход звукового чипа в режим энергосбережения D3",
                OnToggle = state =>
                {
                    HardwareTweaks.SetRealtekLatency(state);
                    OnTweakExecuted?.Invoke("Фикс задержки Realtek", state);
                }
            });

            PerformanceTweaksList.Add(new TweakItem
            {
                Id = "perf_mouse",
                Title = "Отключить акселерацию мыши",
                Description = "Включает чистый 1:1 ввод мыши без встроенного аппаратного ускорения",
                OnToggle = state =>
                {
                    HardwareTweaks.DisableMouseAcceleration(state);
                    OnTweakExecuted?.Invoke("Акселерация мыши", !state);
                }
            });

            PerformanceTweaksList.Add(new TweakItem
            {
                Id = "perf_sticky",
                Title = "Отключить залипание клавиш",
                Description = "Выключает диалоговые окна фильтрации и залипания Shift в играх",
                OnToggle = state =>
                {
                    HardwareTweaks.SetStickyKeys(state);
                    OnTweakExecuted?.Invoke("Залипание клавиш", !state);
                }
            });

            // ================= 3. БЕЗОПАСНОСТЬ =================
            SecurityTweaksList.Add(new TweakItem
            {
                Id = "sec_defender",
                Title = "Отключить Windows Defender",
                Description = "Отключает постоянное сканирование в реальном времени и SmartScreen",
                OnToggle = state =>
                {
                    SecurityTweaks.SetWindowsDefender(state);
                    OnTweakExecuted?.Invoke("Windows Defender", !state);
                }
            });

            SecurityTweaksList.Add(new TweakItem
            {
                Id = "sec_uac",
                Title = "Отключить контроль учетных записей (UAC)",
                Description = "Убирает затемнение экрана и системные всплывающие окна подтверждения",
                OnToggle = state =>
                {
                    SecurityTweaks.SetUac(state);
                    OnTweakExecuted?.Invoke("Контроль UAC", !state);
                }
            });

            SecurityTweaksList.Add(new TweakItem
            {
                Id = "sec_vbs",
                Title = "Отключить изоляцию ядра (VBS)",
                Description = "Снижает задержки процессора за счет отключения виртуализации безопасности",
                OnToggle = state =>
                {
                    SecurityTweaks.SetVbs(state);
                    OnTweakExecuted?.Invoke("Изоляция ядра VBS", !state);
                }
            });

            // ================= 4. СЛУЖБЫ И АПДЕЙТЫ =================
            ServicesTweaksList.Add(new TweakItem
            {
                Id = "svc_unneeded",
                Title = "Отключить фоновые неиспользуемые службы",
                Description = "Останавливает службы факса, карт, отчетов об ошибках и телеметрии",
                OnToggle = state =>
                {
                    ServicesTweaks.SetUnnecessaryServices(state);
                    OnTweakExecuted?.Invoke("Фоновые службы", !state);
                }
            });

            ServicesTweaksList.Add(new TweakItem
            {
                Id = "svc_updates",
                Title = "Приостановить Windows Update",
                Description = "Отключает фоновые проверки и принудительную установку обновлений",
                OnToggle = state =>
                {
                    ServicesTweaks.SetWindowsUpdates(state);
                    OnTweakExecuted?.Invoke("Windows Update", !state);
                }
            });

            ServicesTweaksList.Add(new TweakItem
            {
                Id = "svc_network",
                Title = "Оптимизировать сетевые протоколы",
                Description = "Отключает неиспользуемые Teredo, ISATAP, IPv6 и алгоритм Nagle для игр",
                OnToggle = state =>
                {
                    NetworkTweaks.SetNetworkProtocols(state);
                    if (state) NetworkTweaks.OptimizeGamingLatency();
                    OnTweakExecuted?.Invoke("Оптимизация сети", state);
                }
            });
        }

        public async Task LoadSystemStatsAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    var cpu = CpuGpuMonitor.GetCpuName();
                    var gpu = CpuGpuMonitor.GetGpuName();
                    var (usedRam, totalRam) = RamDiskMonitor.GetRamUsage();
                    var (freeDisk, totalDisk) = RamDiskMonitor.GetSystemDiskSpace();

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        CpuName = cpu;
                        GpuName = gpu;
                        RamUsage = $"{usedRam} GB / {totalRam} GB";
                        DiskSpace = $"{freeDisk} GB / {totalDisk} GB";
                    });
                }
                catch { }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}