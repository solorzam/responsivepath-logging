using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// Indicates the severity of a log entry.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Debugging purposes. Almost certainly should be disabled in production.
        /// </summary>
        Debug,
        /// <summary>
        /// Informational purposes. Not an issue of any kind.
        /// </summary>
        Notice,
        /// <summary>
        /// Minor handled error. Something is clearly going wrong, but did not cause an error for the end user.
        /// </summary>
        Warning,
        /// <summary>
        /// Major handled error. Some kind of error appeared for the end user, but we handled it somehow.
        /// </summary>
        Error,
        /// <summary>
        /// Serious or unhandled error. The user likely noticed something was wrong in a bad way.
        /// </summary>
        FatalError,
    }
}
