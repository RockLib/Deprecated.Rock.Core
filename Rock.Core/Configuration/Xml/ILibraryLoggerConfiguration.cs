using Rock.Logging.Library;

namespace Rock.Configuration.Xml
{
    public interface ILibraryLoggerConfiguration
    {
        bool IsDebugEnabled { get; }
        ILibraryLogger LibraryLogger { get; }
    }
}