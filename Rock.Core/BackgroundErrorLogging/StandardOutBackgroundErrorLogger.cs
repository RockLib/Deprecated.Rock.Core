using System;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="IBackgroundErrorLogger"/> that records log messages to
    /// Standard Out.
    /// </summary>
    public class StandardOutBackgroundErrorLogger : IBackgroundErrorLogger
    {
        /// <summary>
        /// Logs the specified error to Standard Out.
        /// </summary>
        /// <param name="log">The error log.</param>
        public void Log(BackgroundErrorLog log)
        {
            Console.Out.WriteLine(log.Format());
            Console.Out.WriteLine("--------------------------------------------------------------------------------");
        }
    }
}