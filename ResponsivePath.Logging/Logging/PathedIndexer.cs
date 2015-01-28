using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Indexes values from the log entry using JSON.Net's "SelectTokens" syntax.
    /// </summary>
    public class PathedIndexer : ILogIndexer
    {
        private readonly NameValueCollection paths;

        /// <summary>
        /// A collection of paths in the LogEntry and the names to index their values as.
        /// </summary>
        /// <param name="paths">A collection of paths. For instance, you may want to provide "Data.Username" as the key with "User Name" as the value
        /// to create a "User Name" in the indexes, if the property exists in Data.</param>
        public PathedIndexer(NameValueCollection paths)
        {
            this.paths = paths;
        }

        Task ILogIndexer.IndexData(LogEntry logEntry)
        {
            var jsonTokenized = JToken.FromObject(logEntry);
            foreach (var indexKey in paths.AllKeys)
            {
                foreach (var path in paths.GetValues(indexKey))
                {
                    try
                    {
                        var values = jsonTokenized.SelectTokens(path, false);
                        foreach (var value in values)
                        {
                            logEntry.Indexes.Add(indexKey, value.ToString());
                        }
                    }
                    catch { }
                }
            }
            return Task.FromResult<object>(null);
        }
    }
}
