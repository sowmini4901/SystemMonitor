namespace SystemMonitor.DataContracts
{
    /// <summary>
    /// Monitoring Metrics
    /// </summary>
    public class MonitoringMetrics
    {
        #region Public Members

        /// <summary>
        /// IntervalSeconds
        /// </summary>
        public int IntervalSeconds { get; set; }

        /// <summary>
        /// ApiUrl
        /// </summary>
        public string ApiUrl { get; set; }

        #endregion
    }
}
