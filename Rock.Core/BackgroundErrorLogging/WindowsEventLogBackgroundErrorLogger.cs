using System.Diagnostics;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="IBackgroundErrorLogger"/> that records log messages to
    /// the Windows Event Log.
    /// </summary>
    public class WindowsEventLogBackgroundErrorLogger : IBackgroundErrorLogger
    {
        /// <summary>
        /// Gets the Windows Event Log source name by which the application is registered on the local computer.
        /// </summary>
        protected virtual string Source
        {
            get { return ".NET Runtime"; }
        }

        /// <summary>
        /// Gets the name of the Windows Event Log that the source's entries are written to. Possible values include Application,
        /// System, or a custom event log.
        /// </summary>
        protected virtual string LogName
        {
            get { return "Application"; }
        }

        /// <summary>
        /// Logs the specified error to the Windows Event Log.
        /// </summary>
        /// <param name="log">The error log.</param>
        public void Log(BackgroundErrorLog log)
        {
            if (!EventLog.SourceExists(Source))
            {
                EventLog.CreateEventSource(Source, LogName);
            }

            var eventLog = new EventLog("Application")
            {
                Source = Source,
            };

            eventLog.WriteEntry(log.Format(), EventLogEntryType.Error);
        }
    }
}