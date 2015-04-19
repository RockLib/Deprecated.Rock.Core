using System;

namespace Rock.Logging.Library
{
    public interface ILibraryLogger : IDisposable
    {
        void Log(ILibraryLogMessage message);
        void Debug(ILibraryLogMessage message); 
    }
}