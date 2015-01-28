using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// A basic entry describing a single log event
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LogEntry()
        {
            Timestamp = DateTimeOffset.Now;
            Data = new Dictionary<string, object>();
            Indexes = new NameValueCollection();
        }

        /// <summary>
        /// Gets or sets the timestamp for the log entry. Defaults to DateTimeOffset.Now.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the severity for the log entry.
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        /// Gets or sets the human-readable message for the log entry. Should not contain any account numbers, etc; these should be placed in the Data container.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the exception that is the reason for the log entry.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets a data container, which should have data useful to a debugger.  This may include customer or account numbers, HTTP requests, etc.
        /// </summary>
        public IDictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Gets a set of indexes, which should be searchable on the recorded mediums. This might include session ids, usernames, reference numbers,
        /// or other data to assist in locating a problem.
        /// </summary>
        public NameValueCollection Indexes { get; private set; }
    }
}
