using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Extensions for creating log entries more fluidly
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Records a log entry without an exception.
        /// </summary>
        /// <param name="logger">The logger to which the entry is recorded.</param>
        /// <param name="message">The human-readable message for the log entry. Should not contain any variable values.</param>
        /// <param name="severity">The severity of the entry to record.</param>
        /// <param name="data">Additional data to record with the entry, such as account numbers and other variables useful for debugging. Optional.</param>
        /// <returns>A task representing the progress of the action.</returns>
        public static Task Record(this ILogger logger, string message, Severity severity, Dictionary<string, object> data = null)
        {
            return logger.Record(CopyData(new LogEntry { Message = message, Severity = severity }, data));
        }

        /// <summary>
        /// Records a log entry with an exception
        /// </summary>
        /// <param name="logger">The logger to which the entry is recorded.</param>
        /// <param name="message">The human-readable message for the log entry. Should not contain any variable values.</param>
        /// <param name="exception">The exception to associate with the log entry.</param>
        /// <param name="severity">The severity of the entry to record. Optional, defaults to Error.</param>
        /// <param name="data">Additional data to record with the entry, such as account numbers and other variables useful for debugging. Optional.</param>
        /// <returns>A task representing the progress of the action.</returns>
        public static Task Record(this ILogger logger, string message, Exception exception, Severity severity = Severity.Error, Dictionary<string, object> data = null)
        {
            return logger.Record(CopyData(new LogEntry { Message = message, Severity = severity, Exception = exception }, data));
        }


        private static LogEntry CopyData(LogEntry logEntry, Dictionary<string, object> data)
        {
            if (data != null)
            {
                foreach (var key in data.Keys)
                {
                    logEntry.Data[key] = data[key];
                }
            }

            return logEntry;
        }
    }
}
