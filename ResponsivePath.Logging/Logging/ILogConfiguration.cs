using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// A basic interface to configure a logger.
    /// </summary>
    public interface ILogConfiguration
    {
        /// <summary>
        /// The data accumulators to execute for each log entry.
        /// </summary>
        IEnumerable<IDataAccumulator> Accumulators { get; }

        /// <summary>
        /// The indexers to run for each log entry.
        /// </summary>
        IEnumerable<ILogIndexer> Indexers { get; }

        /// <summary>
        /// The log recorders
        /// </summary>
        IEnumerable<ILogRecorder> Recorders { get; }

        /// <summary>
        /// True if the log recorder should block until the log is complete; setting this to false will run Indexers and Recorders without awaiting.
        /// </summary>
        bool WaitForLogRecording { get; }
    }
}
