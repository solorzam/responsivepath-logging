using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Simple interface to record a log entry
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Records a log entry
        /// </summary>
        /// <param name="logEntry">The log entry to record</param>
        /// <returns>A task representing the progress of the action.</returns>
        Task Record(LogEntry logEntry);
    }
}
