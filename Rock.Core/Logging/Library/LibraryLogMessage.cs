using System;
using System.Runtime.CompilerServices;

namespace Rock.Logging.Library
{
    public class LibraryLogMessage : ILibraryLogMessage
    {
        public LibraryLogMessage(
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            CreateTime = DateTime.UtcNow;
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }

        public string LibraryName { get; set; }
        public string Message { get; set; }
        public DateTime CreateTime { get; set; }
        public Exception Exception { get; set; }
        public string CallerMemberName { get; set; }
        public string CallerFilePath { get; set; }
        public int CallerLineNumber { get; set; }
    }
}