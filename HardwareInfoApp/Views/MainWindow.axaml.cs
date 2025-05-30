using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HardwareInfoApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnGetInfoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Операційна система: {RuntimeInformation.OSDescription}");
            sb.AppendLine($"Архітектура: {RuntimeInformation.OSArchitecture}");
            sb.AppendLine($"Процесор: {await RunCommand("sysctl -n machdep.cpu.brand_string")}");
            sb.AppendLine($"Модель Mac: {await RunCommand("sysctl -n hw.model")}");
            sb.AppendLine($"Кількість ядер: {Environment.ProcessorCount}");
            sb.AppendLine($"Пам’ять (RAM): {await RunCommand("sysctl -n hw.memsize")} байт");
            sb.AppendLine();

            sb.AppendLine("Диск:");
            sb.AppendLine(await RunCommand("df -h /"));

            sb.AppendLine("Відеокарта:");
            sb.AppendLine(await RunCommand("system_profiler SPDisplaysDataType | grep 'Chipset Model'"));

            sb.AppendLine("Мережеві інтерфейси:");
            sb.AppendLine(await RunCommand("networksetup -listallhardwareports"));

            sb.AppendLine("Серійний номер:");
            sb.AppendLine(await RunCommand("system_profiler SPHardwareDataType | grep 'Serial Number'"));

            InfoTextBox.Text = sb.ToString();
        }

        private async Task<string> RunCommand(string command)
        {
            try
            {
                var processInfo = new ProcessStartInfo("bash", $"-c \"{command}\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(processInfo);
                string result = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                return result.Trim();
            }
            catch (Exception ex)
            {
                return $"Помилка: {ex.Message}";
            }
        }
    }
}
