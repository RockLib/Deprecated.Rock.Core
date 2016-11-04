using System;

namespace Rock.Serialization
{
    /// <summary>
    /// An exception that is thrown when an invalid xml is encountered when creating an
    /// instance of an object through an xml deserialization proxy.
    /// </summary>
    public class XmlDeserializationProxyException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDeserializationProxyException"/>
        /// class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public XmlDeserializationProxyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}