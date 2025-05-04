namespace SystemMonitor.Contracts
{
    /// <summary>
    /// ISystemResourceMonitor interface
    /// </summary>
    public interface ISystemResourceMonitor
    {
        #region Public Methods

        /// <summary>
        /// Registering the plugins
        /// </summary>
        /// <param name="plugin">IMonitor plugin</param>
        void RegisterPlugin(IMonitorPlugin plugin);

        /// <summary>
        /// Starting the monitoring
        /// </summary>
        void Start();

        /// <summary>
        /// Stopping the monitoring
        /// </summary>
        void Stop();
        /// <summary>
        /// Getting System CPU Usage
        /// </summary>
        float GetSystemCPUUsage();

        /// <summary>
        /// Getting RAM usage
        /// </summary>
        float GetRAMUsage();

        /// <summary>
        /// Getting System disk usage
        /// </summary>
        float GetSystemDiskUsage();

        #endregion
    }
}
