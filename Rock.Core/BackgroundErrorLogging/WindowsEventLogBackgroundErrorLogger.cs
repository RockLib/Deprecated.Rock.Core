using System;
using System.Diagnostics;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="IBackgroundErrorLogger"/> that records log messages to
    /// the Windows Event Log.
    /// </summary>
    public class WindowsEventLogBackgroundErrorLogger : IBackgroundErrorLogger
    {
        private readonly Lazy<Action<BackgroundErrorLog>> _logAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsEventLogBackgroundErrorLogger"/> class.
        /// </summary>
        public WindowsEventLogBackgroundErrorLogger()
        {
            _logAction = new Lazy<Action<BackgroundErrorLog>>(() =>
            {
                try
                {
                    if (!EventLog.SourceExists(Source))
                    {
                        EventLog.CreateEventSource(Source, LogName);
                    }

                    var eventLog = new EventLog(LogName)
                    {
                        Source = Source,
                    };

                    var eventLogEntryType = EventLogEntryType;

                    return log => eventLog.WriteEntry(log.Format(), eventLogEntryType);
                }
                catch
                {
                    return log => {};
                }
            });
        }

        /// <summary>
        /// Gets the Windows Event Log source name by which the application is registered on the local computer.
        /// </summary>
        protected virtual string Source
        {
            // We need a good default value for the event log source - one that should
            // "always exist". Unless an application is running as Administrator, when
            // a source does not exist, a call to EventLog.SourceExists will throw a
            // SecurityException. We want to avoid this. Since whatever is running this
            // code *is* a .NET application (obviously), the ".NET Runtime" source
            // *should* exist.
            get { return ".NET Runtime"; }
        }

        /// <summary>
        /// Gets the name of the Windows Event Log that the source's entries are written to. Possible values include Application,
        /// System, or a custom event log.
        /// </summary>
        protected virtual string LogName
        {
            // The ".NET Runtime" source exists in the "Application" log.
            get { return "Application"; }
        }

        /// <summary>
        /// Gets the event type to use when writing to the Windows Event Log.
        /// </summary>
        protected virtual EventLogEntryType EventLogEntryType
        {
            get { return EventLogEntryType.Error; }
        }

        /// <summary>
        /// Logs the specified error to the Windows Event Log.
        /// </summary>
        /// <param name="log">The error log.</param>
        public void Log(BackgroundErrorLog log)
        {
            _logAction.Value(log);
        }
    }
}