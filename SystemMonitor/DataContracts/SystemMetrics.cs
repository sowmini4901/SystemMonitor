namespace SystemMonitor.DataContracts
{
    /// <summary>
    /// SystemMetrics class
    /// </summary>
    public class SystemMetrics
    {
        #region Public Members

        /// <summary>
        /// cpu usage in percentage
        /// </summary>
        public float CpuUsage { get; set; }

        /// <summary>
        /// private ram used in mb
        /// </summary>
        public float PrivateRamUsedMb { get; set; }

        /// <summary>
        /// disk usage in mb
        /// </summary>
        public float DiskUsedGb { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"CPU: {CpuUsage:F2}% | RAM: {PrivateRamUsedMb:F2} MB | Disk: {DiskUsedGb:F2} GB";
        }

        #endregion
    }
}
