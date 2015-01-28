using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    class StackTraceIndexer : IDataAccumulator
    {
        private Regex[] stackTraceReplacements;

        /// <param name="stackTraceReplacements">
        /// A set of regular expressions to filter out parts from the stack trace.  For example, @" in (.*)\\ResponsivePath" 
        /// will filter out anything before the "ReponsivePath" namespace, causing local build directories to be the same.
        /// </param>
        public StackTraceIndexer(string[] stackTraceReplacements)
        {
            this.stackTraceReplacements = stackTraceReplacements.Select(replacement => new Regex(replacement, RegexOptions.Compiled)).ToArray();
        }

        Task IDataAccumulator.AccumulateData(LogEntry logEntry)
        {
            string stackTrace;
            if (logEntry.Exception == null)
            {
                stackTrace = Environment.StackTrace;
            }
            else
            {
                StringBuilder stackTraceBuilder = new StringBuilder();
                Stack<Exception> stack = new Stack<Exception>();
                var ex = logEntry.Exception;
                while (ex != null)
                {
                    stack.Push(ex);
                    ex = ex.InnerException;
                }
                do
                {
                    ex = stack.Pop();
                    stackTraceBuilder.AppendLine(ex.StackTrace);
                } while (stack.Count > 0);
                stackTrace = stackTraceBuilder.ToString();
            }

            foreach (var regex in stackTraceReplacements)
            {
                stackTrace = regex.Replace(stackTrace, "");
            }

            logEntry.Data["StackTrace"] = stackTrace;
            var hash = Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(stackTrace)));
            logEntry.Data["StackTraceHash"] = hash;

            return Task.FromResult<object>(null);
        }
    }
}
