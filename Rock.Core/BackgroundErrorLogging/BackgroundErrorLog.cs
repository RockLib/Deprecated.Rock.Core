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
        private string _dateTimeFormat;

        // ReSharper disable ExplicitCallerInfoArgument
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
            : this(() => DateTime.UtcNow, callerMemberName, callerFilePath, callerLineNumber)
        {
        }
        // ReSharper restore ExplicitCallerInfoArgument

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundErrorLog"/> class. The <see cref="CreateTime"/>
        /// property is initialized as the value returned by the <paramref name="getNow"/> function.
        /// </summary>
        /// <param name="getNow">A function that returns a <see cref="DateTime"/> representing "now".</param>
        /// <param name="callerMemberName">The method or property name of the caller to a background error logger method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller to a background error logger method.</param>
        /// <param name="callerLineNumber">The line number in the source file at which a background error logger method is called.</param>
        public BackgroundErrorLog(
            Func<DateTime> getNow,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (getNow == null) throw new ArgumentNullException("getNow");

            CreateTime = getNow();
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;

            _dateTimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss'.'fffK";
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
        /// Gets or sets additional information about the error log.
        /// </summary>
        public string AdditionalInformation { get; set; }

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

        /// <summary>
        /// Sets the <see cref="DateTime"/> format used to convert <see cref="CreateTime"/> to a string.
        /// This value is used by the <see cref="Format"/> method.
        /// </summary>
        /// <param name="format">A <see cref="DateTime"/> format string.</param>
        public void SetDateTimeFormat(string format)
        {
            if (format == null) throw new ArgumentNullException("format");

            _dateTimeFormat = format;
        }

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

            sb.AppendFormat("ERROR: {0} {1}", CreateTime.ToString(_dateTimeFormat), LibraryName).AppendLine();

            if (message != null)
            {
                sb.AppendFormat("    {0}", message).AppendLine();
            }

            sb.AppendFormat("    {0}:{1}({2})", CallerFilePath, CallerMemberName, CallerLineNumber);

            if (Exception != null)
            {
                sb.AppendLine().Append(Exception.FormatToString());
            }

            if (AdditionalInformation != null)
            {
                sb.AppendLine().Append(AdditionalInformation);
            }

            return sb.ToString();
        }
    }
}