using System;
using System.Configuration;
using System.Runtime.Serialization;
using System.Xml;

namespace Rock.Configuration
{
    [Serializable]
    public class InvalidConfigurationException : ConfigurationException
    {
        public InvalidConfigurationException(string message, XmlNode node)
            : base(message, node)
        {
        }

        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
