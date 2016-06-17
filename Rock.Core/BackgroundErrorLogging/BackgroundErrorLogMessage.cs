using System;
using System.Runtime.CompilerServices;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// Defines a log message for a background error.
    /// </summary>
    public class BackgroundErrorLogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundErrorLogMessage"/> class.
        /// </summary>
        public BackgroundErrorLogMessage(
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            CreateTime = DateTime.UtcNow;
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }

        /// <summary>
        /// Gets or sets the name of the library where the log message is coming from.
        /// </summary>
        public string LibraryName { get; set; }

        /// <summary>
        /// Gets or sets the message of the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the time that the log message was created.
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the exception associated with the log message.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the method or property name where the log message originated.
        /// </summary>
        public string CallerMemberName { get; set; }

        /// <summary>
        /// Gets or sets the full path of the source file where the log message originated.
        /// </summary>
        public string CallerFilePath { get; set; }

        /// <summary>
        /// Gets or sets the line number in the source file where the log message originated.
        /// </summary>
        public int CallerLineNumber { get; set; }
    }
}