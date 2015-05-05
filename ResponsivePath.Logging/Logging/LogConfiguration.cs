using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Basic ILogConfiguration implementation
    /// </summary>
    public class LogConfiguration : ILogConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LogConfiguration()
        {
            Accumulators = new List<IDataAccumulator>();
            Indexers = new List<ILogIndexer>();
            Recorders = new List<ILogRecorder>();
        }

        IEnumerable<IDataAccumulator> ILogConfiguration.Accumulators
        {
            get { return Accumulators; }
        }

        IEnumerable<ILogIndexer> ILogConfiguration.Indexers
        {
            get { return Indexers; }
        }

        IEnumerable<ILogRecorder> ILogConfiguration.Recorders
        {
            get { return Recorders; }
        }

        /// <summary>
        /// A list of data accumulators to use with the logger
        /// </summary>
        public IList<IDataAccumulator> Accumulators { get; private set; }

        /// <summary>
        /// A list of indexers to use with the logger
        /// </summary>
        public IList<ILogIndexer> Indexers { get; private set; }

        /// <summary>
        /// A list of recorders for each log entry
        /// </summary>
        public IList<ILogRecorder> Recorders { get; private set; }

        /// <summary>
        /// True if the log recorder should block until the log is complete; setting this to false will run Indexers and Recorders without awaiting. Defaults to false.
        /// </summary>
        public bool WaitForLogRecording { get; set; }
    }
}
