using Newtonsoft.Json;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;

using SystemMonitor.Contracts;
using SystemMonitor.DataContracts;

namespace SystemMonitor.SystemResourceMonitor
{
    /// <summary>
    /// Windows System Resource Monitor
    /// </summary>
    public class WindowsSystemResourceMonitor : ISystemResourceMonitor, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public WindowsSystemResourceMonitor(int intervalNo, string urlPath)
        {
            interval = intervalNo;
            apiPath = urlPath;
            cancellationToken = new CancellationTokenSource();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void RegisterPlugin(IMonitorPlugin plugin)
        {
            listOfPlugins.Add(plugin);
        }

        /// <inheritdoc />
        public void Start()
        {
            Task.Factory.StartNew(CalculateSystemMetrics, TaskCreationOptions.LongRunning);
        }

        /// <inheritdoc />
        public void Stop()
        {
            cancellationToken.Cancel();
        }

        /// <inheritdoc />
        public float GetRAMUsage()
        {
            var output = RunCommand("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            ulong totalKB = 0, freeKB = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("TotalVisibleMemorySize"))
                    totalKB = ulong.Parse(line.Split('=')[1]);
                else if (line.StartsWith("FreePhysicalMemory"))
                    freeKB = ulong.Parse(line.Split('=')[1]);
            }

            float totalMB = totalKB / 1024.0f;
            float usedMB = (totalKB - freeKB) / 1024.0f;
            return usedMB/totalMB;
        }

        /// <inheritdoc />
        public float GetSystemCPUUsage()
        {
            using (PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total")) {
                cpuCounter.NextValue();
                Task.Delay(delay).Wait();
                return cpuCounter.NextValue();
            }
        }

        /// <inheritdoc />
        public float GetSystemDiskUsage()
        {
            var drives = DriveInfo.GetDrives();
            float total = 0, used = 0;

            foreach (var drive in drives)
            {
                if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                {
                    float driveTotal = drive.TotalSize / 1024.0f / 1024 / 1024;
                    float driveUsed = (drive.TotalSize - drive.TotalFreeSpace) / 1024.0f / 1024 / 1024;
                    total += driveTotal;
                    used += driveUsed;
                }
            }

            return used/total;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void CalculateSystemMetrics()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    float cpuUsage = GetSystemCPUUsage();
                    float diskUsage = GetSystemDiskUsage();
                    float ramUsage = GetRAMUsage();
                    SystemMetrics systemMetrics = new SystemMetrics
                    {
                        CpuUsage = cpuUsage,
                        DiskUsedGb = diskUsage,
                        PrivateRamUsedMb = ramUsage
                    };

                    //updating in the plugins available
                    foreach (var plugin in listOfPlugins)
                    {
                        plugin.OnUpdate(systemMetrics);
                    }
                    //updating the data to console output
                    Console.WriteLine($"CPU: {cpuUsage:F2}% | RAM: {ramUsage:F2} MB | Disk: {diskUsage:F2} GB");

                    //Posting the data to REST API
                    PostingToAPI(systemMetrics);
                    Task.Delay(interval*1000).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    cancellationToken.Cancel(true);
                }
            }
        }

        private void PostingToAPI(SystemMetrics metrics)
        {
            var payload = new
            {
                cpu = Math.Round(metrics.CpuUsage, 2),
                ram_used = Math.Round(metrics.PrivateRamUsedMb, 2),
                disk_used = Math.Round(metrics.DiskUsedGb, 2)
            };

            Console.WriteLine(JsonConvert.SerializeObject(payload));

            try
            {
                var response =  httpClient.PostAsJsonAsync(apiPath, payload);
                if(response.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine($"Posted at {DateTime.Now}");
                }
                else
                {
                    Console.WriteLine($"Failed to post to the REST API");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to post: {ex.Message}");
            }
        }

        private string RunCommand(string filename, string args)
        {
            //process will be disposed in this block 
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                return output;
            };
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    cancellationToken?.Dispose();
                    httpClient.Dispose();
                    listOfPlugins.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        #endregion

        #region Private Members

        private readonly ConcurrentBag<IMonitorPlugin> listOfPlugins = new ConcurrentBag<IMonitorPlugin>();
        private CancellationTokenSource cancellationToken;
        private bool disposedValue;
        private int interval;
        private string apiPath;
        private HttpClient httpClient = new HttpClient();
        private int delay = 500;

        #endregion

    }
}
