using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Adds the current stack trace, or the stack trace from the exception if one is provided, to the Data. Also, adds a "StackTraceHash"
    /// to identify similar issues.
    /// </summary>
    public class StackTraceAccumulator : IDataAccumulator
    {
        private readonly Regex[] stackTraceReplacements;
        private readonly int frameSkip;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="frameSkip">The number of stack trace frames to skip when pulling from Environment.StackTrace.</param>
        /// <param name="stackTraceReplacements">
        /// A set of regular expressions to filter out parts from the stack trace.  For example, @" in (.*)\\ResponsivePath" 
        /// will filter out anything before the "ReponsivePath" namespace, causing local build directories to be the same.
        /// </param>
        public StackTraceAccumulator(int frameSkip, string[] stackTraceReplacements)
        {
            this.frameSkip = frameSkip;
            this.stackTraceReplacements = stackTraceReplacements.Select(replacement => new Regex(replacement, RegexOptions.Compiled)).ToArray();
        }

        void IDataAccumulator.AccumulateData(LogEntry logEntry)
        {
            string stackTrace;
            if (logEntry.Exception == null)
            {
                stackTrace = string.Join("\n", Environment.StackTrace.Split('\n').Skip(3 + frameSkip));
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
                var replaced = regex.Match(stackTrace).Groups[1].Value;
                stackTrace = stackTrace.Replace(replaced, "");
            }

            logEntry.Data["StackTrace"] = stackTrace;
            var hash = Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(stackTrace)));
            logEntry.Data["StackTraceHash"] = hash;
        }
    }
}
