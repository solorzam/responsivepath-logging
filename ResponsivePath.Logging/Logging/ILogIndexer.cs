using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Adds additional index data to a log entry based on other properties, either in the log entry itself or from the environment at large
    /// </summary>
    public interface ILogIndexer
    {
        /// <summary>
        /// Writes indexes to the log entry.
        /// </summary>
        /// <param name="logEntry">The log entry to index.</param>
        /// <returns>A task representing the progress of the action.</returns>
        Task IndexData(LogEntry logEntry);
    }
}
