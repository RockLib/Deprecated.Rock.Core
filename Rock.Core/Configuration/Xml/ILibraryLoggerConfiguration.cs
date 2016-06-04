using Rock.LibraryLogging;

namespace Rock.Configuration.Xml
{
    public interface ILibraryLoggerConfiguration
    {
        bool IsDebugEnabled { get; }
        ILibraryLogger LibraryLogger { get; }
    }
}