#region Using Directives

using Newtonsoft.Json;
using System.Runtime.InteropServices;
using SystemMonitor.DataContracts;
using SystemMonitor.Plugins;
using SystemMonitor.SystemResourceMonitor;

#endregion

public class Program
{
    public static void Main()
    {
        //for windows platform
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Cfg", "UrlSettings.json");
            string jsonContent = File.ReadAllText(filePath);
            //For posting the collected system resource data to REST API
            var configData = JsonConvert.DeserializeObject<MonitoringMetrics>(jsonContent);
            if (configData != null) {
                var monitor = new WindowsSystemResourceMonitor(configData.IntervalSeconds, configData.ApiUrl);
                monitor.RegisterPlugin(new FileMonitorPlugin(@"D:/metrics_log.txt"));
                monitor.Start();
            }
            
            //todo can call monitor.stop() 
            //to stop the monitoring
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            //TODO for other platforms
        }

        Console.WriteLine("Monitoring started. Press Enter to exit.");
        Console.ReadLine();
    }
}
