using System;
using System.Runtime.CompilerServices;
using System.Text;
using Rock.StringFormatting;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// Defines a log for a background error.
    /// </summary>
    public class BackgroundErrorLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundErrorLog"/> class. The <see cref="CreateTime"/>
        /// property is initialized as <see cref="DateTime.UtcNow"/>.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to a background error logger method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller to a background error logger method.</param>
        /// <param name="callerLineNumber">The line number in the source file at which a background error logger method is called.</param>
        public BackgroundErrorLog(
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
        /// Gets or sets the name of the library where the error log originated.
        /// </summary>
        public string LibraryName { get; set; }

        /// <summary>
        /// Gets or sets the message of the error log.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the time that the error log was created.
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the exception associated with the error log.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the method or property name where the error log originated.
        /// </summary>
        public string CallerMemberName { get; set; }

        /// <summary>
        /// Gets or sets the full path of the source file where the error log originated.
        /// </summary>
        public string CallerFilePath { get; set; }

        /// <summary>
        /// Gets or sets the line number in the source file where the error log originated.
        /// </summary>
        public int CallerLineNumber { get; set; }

        public virtual string Format()
        {
            var sb = new StringBuilder();

            string message;

            if (Message != null)
            {
                message = Message;
            }
            else if (Exception != null)
            {
                message = Exception.Message;
            }
            else
            {
                message = null;
            }

            sb.AppendFormat("ERROR: {0:O} {1}", CreateTime, LibraryName).AppendLine();

            if (message != null)
            {
                sb.AppendFormat("    {0}", message).AppendLine();
            }

            sb.AppendFormat("    {0}:{1}({2})", CallerFilePath, CallerMemberName, CallerLineNumber);

            if (Exception != null)
            {
                sb.AppendLine().Append(Exception.FormatToString());
            }

            return sb.ToString();
        }
    }
}