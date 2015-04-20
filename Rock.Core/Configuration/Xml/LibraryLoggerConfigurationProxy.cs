using Rock.Serialization;

namespace Rock.Configuration.Xml
{
    public class LibraryLoggerConfigurationProxy : XmlDeserializationProxy<ILibraryLoggerConfiguration>
    {
        public LibraryLoggerConfigurationProxy()
            : base(typeof(LibraryLoggerConfiguration))
        {
        }
    }
}