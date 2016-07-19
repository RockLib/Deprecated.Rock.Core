using System;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="IBackgroundErrorLogger"/> that records log messages to
    /// Standard Error.
    /// </summary>
    public class StandardErrorBackgroundErrorLogger : IBackgroundErrorLogger
    {
        /// <summary>
        /// Logs the specified error to Standard Error.
        /// </summary>
        /// <param name="log">The error log.</param>
        public void Log(BackgroundErrorLog log)
        {
            Console.Error.WriteLine(log.Format());
            Console.Error.WriteLine("--------------------------------------------------------------------------------");
        }
    }
}