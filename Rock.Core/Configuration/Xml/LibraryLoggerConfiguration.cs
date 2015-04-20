using Rock.Logging.Library;

namespace Rock.Configuration.Xml
{
    public class LibraryLoggerConfiguration : ILibraryLoggerConfiguration
    {
        public bool IsDebugEnabled { get; set; }
        public ILibraryLogger LibraryLogger { get; set; }
    }
}