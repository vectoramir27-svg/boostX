using System;
using System.Windows;

namespace BoostX
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                MessageBox.Show($"Критическая ошибка:\n{args.ExceptionObject}", "BoostX Crash", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Ошибка интерфейса:\n{args.Exception.Message}\n\nПодробности:\n{args.Exception.InnerException?.Message}", "BoostX WPF Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}