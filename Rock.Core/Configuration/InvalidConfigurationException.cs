using System;
using System.Configuration;
using System.Runtime.Serialization;
using System.Xml;

namespace Rock.Configuration
{
    /// <summary>
    /// The exception that is thrown when an invalid configuration is encountered.
    /// </summary>
    [Serializable]
    public class InvalidConfigurationException : ConfigurationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="message">A message describing why this <see cref="InvalidConfigurationException"/> exception was thrown.</param>
        /// <param name="node">The <see cref="T:System.Xml.XmlNode"/> that caused this <see cref="InvalidConfigurationException"/> to be thrown.</param>
        public InvalidConfigurationException(string message, XmlNode node)
            : base(message, node)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the information to deserialize.</param>
        /// <param name="context">Contextual information about the source or destination.</param>
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
