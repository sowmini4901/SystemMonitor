#region Using Directives

using SystemMonitor.Contracts;
using SystemMonitor.DataContracts;

#endregion

namespace SystemMonitor.Plugins
{
    /// <summary>
    /// FileMonitorPlugin class
    /// </summary>
    public class FileMonitorPlugin : IMonitorPlugin
    {
        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="filePathInstance"></param>
        public FileMonitorPlugin(string filePathInstance)
        {
            filePath = filePathInstance;
        }

        #endregion

        #region Public Methods

        public void OnUpdate(SystemMetrics metrics)
        {
            File.AppendAllText(filePath, metrics.ToString() + "\n");
        }

        #endregion

        #region private Members

        private readonly string filePath;

        #endregion
    }
}
