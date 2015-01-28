using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Records a log entry to a persistence layer.
    /// </summary>
    public interface ILogRecorder
    {
        /// <summary>
        /// Records a log entry to a persistence layer.
        /// </summary>
        /// <param name="logEntry">The log entry to record.</param>
        /// <returns>A task representing the progress of the action.</returns>
        Task Save(LogEntry logEntry);
    }
}
