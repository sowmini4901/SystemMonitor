#region Using Directives

using SystemMonitor.DataContracts;

#endregion

namespace SystemMonitor.Contracts
{
    /// <summary>
    /// IMonitorPlugin
    /// </summary>
    public interface IMonitorPlugin
    {
        #region Public Methods

        /// <summary>
        /// On updating the metrics
        /// </summary>
        /// <param name="metrics">system metrics</param>
        void OnUpdate(SystemMetrics metrics);

        #endregion
    }
}
