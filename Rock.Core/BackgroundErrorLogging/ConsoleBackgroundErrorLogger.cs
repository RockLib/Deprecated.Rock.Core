using System;
using Rock.StringFormatting;

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
            Console.Write("LOG");
            WriteMessage(message);
        }

        private static void WriteMessage(BackgroundErrorLogMessage message)
        {
            Console.WriteLine(" {0:O} {1}", message.CreateTime, message.LibraryName);
            Console.WriteLine("    {0}", message.Message);
            Console.WriteLine("    {0}:{1}({2})", message.CallerFilePath, message.CallerMemberName, message.CallerLineNumber);

            if (message.Exception != null)
            {
                Console.WriteLine(message.Exception.FormatToString());
            }

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}