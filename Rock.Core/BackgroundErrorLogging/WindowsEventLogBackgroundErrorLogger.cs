using System.Diagnostics;

namespace Rock.BackgroundErrorLogging
{
    public class WindowsEventLogBackgroundErrorLogger : IBackgroundErrorLogger
    {
        protected virtual string Source
        {
            get { return ".NET Runtime"; }
        }

        protected virtual string LogName
        {
            get { return "Application"; }
        }

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