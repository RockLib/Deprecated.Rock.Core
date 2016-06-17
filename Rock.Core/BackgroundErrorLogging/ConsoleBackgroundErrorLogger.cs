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
        /// <param name="message">The message.</param>
        public void Log(BackgroundErrorLogMessage message)
        {
            Console.Out.WriteLine(message.Format());
            Console.Out.WriteLine();
            Console.Out.WriteLine("--------------------------------------------------------------------------------");
            Console.Out.WriteLine();
        }
    }
}