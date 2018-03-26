using Rock.Serialization;

namespace Rock.DataProtection.Xml
{
    /// <summary>
    /// A class that creates instances of <see cref="IProtectedValue"/>.
    /// </summary>
    public class ProtectedValueProxy : XmlDeserializationProxy<IProtectedValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedValueProxy"/> class.
        /// </summary>
        public ProtectedValueProxy()
            : base(typeof(UnprotectedValue))
        {
        }
    }
}