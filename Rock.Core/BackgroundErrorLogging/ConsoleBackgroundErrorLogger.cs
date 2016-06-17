using System;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="IBackgroundErrorLogger"/> that records log messages with
    /// the <see cref="Console"/> class.
    /// </summary>
    public class ConsoleBackgroundErrorLogger : IBackgroundErrorLogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="log">The error log.</param>
        public void Log(BackgroundErrorLog log)
        {
            Console.Out.WriteLine(log.Format());
            Console.Out.WriteLine();
            Console.Out.WriteLine("--------------------------------------------------------------------------------");
            Console.Out.WriteLine();
        }
    }
}