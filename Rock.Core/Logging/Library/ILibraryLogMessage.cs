using System;

namespace Rock.Logging.Library
{
    public interface ILibraryLogMessage
    {
        string LibraryId { get; }
        string Message { get; }
        DateTime CreateTime { get; }
        Exception Exception { get; }
        string CallerMemberName { get; }
        string CallerFilePath { get; }
        int CallerLineNumber { get; }
    }
}