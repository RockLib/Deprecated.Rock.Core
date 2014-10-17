using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Rock.Configuration
{
    /// <summary>
    /// A section handler that creates an instance of a type based
    /// on an attribute named "type" defined in a section element 
    /// from a *.config file, using the an instance of
    /// <see cref="XmlSerializer"/> to deserialize the element.
    /// </summary>
    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return Deserialize(section, GetConfigType(section));
        }

        /// <summary>
        /// Deserialize the configuration object that will be returned by the
        /// <see cref="Create"/> method.
        /// </summary>
        /// <param name="section">An xml node that represents configuration object in the *.config file.</param>
        /// <param name="configType">The type of the object to be returned by the <see cref="Create"/> method.</param>
        /// <returns>The deserialized object.</returns>
        protected virtual object Deserialize(XmlNode section, Type configType)
        {
            var serializer = new XmlSerializer(configType, new XmlRootAttribute(section.Name));

            using (var reader = new StringReader(section.OuterXml))
            {
                return serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Get the type of the configuration object that will be returned by the
        /// <see cref="Create"/> method.
        /// </summary>
        /// <param name="section">An xml node that represents configuration object in the *.config file.</param>
        /// <returns>The type of the configuration object that will be returned by the <see cref="Create"/> method.</returns>
        protected virtual Type GetConfigType(XmlNode section)
        {
            var xPathNavigator = section.CreateNavigator();
            var typeName = (string)xPathNavigator.Evaluate("string(@type)");

            if (typeName == null)
            {
                throw new InvalidConfigurationException(string.Format("A type must be provided for section '{0}'.", section.Name), section);
            }

            var configType = Type.GetType(typeName);

            if (configType == null)
            {
                throw new InvalidConfigurationException(string.Format("The specified type, '{0}', is invalid for section '{1}' (use the type's assembly qualified name).", typeName, section.Name), section);
            }

            return configType;
        }
    }

    /// <summary>
    /// A section handler that creates an instance of the 
    /// <typeparamref name="TConfiguration"/> type, using the an instance of
    /// <see cref="XmlSerializer"/> to deserialize the element.
    /// </summary>
    public class XmlSerializerSectionHandler<TConfiguration> : XmlSerializerSectionHandler
    {
        /// <summary>
        /// Get the type of the configuration object that will be returned by the
        /// <see cref="XmlSerializerSectionHandler.Create"/> method.
        /// </summary>
        /// <param name="section">An xml node that represents configuration object in the *.config file.</param>
        /// <returns>The type of the configuration object that will be returned by the <see cref="XmlSerializerSectionHandler.Create"/> method.</returns>
        protected override Type GetConfigType(XmlNode section)
        {
            return typeof(TConfiguration);
        }
    }
}
