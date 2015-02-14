using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Accumulates additional data to a log entry based on information in the environment at large. This could include session information, machine configuration, http requests, etc.
    /// </summary>
    public interface IDataAccumulator
    {
        /// <summary>
        /// Writes data to a log entry.
        /// </summary>
        /// <param name="logEntry">The log entry for which data should be accumulated.</param>
        /// <returns>A task representing the progress of the action.</returns>
        void AccumulateData(LogEntry logEntry);
    }
}
