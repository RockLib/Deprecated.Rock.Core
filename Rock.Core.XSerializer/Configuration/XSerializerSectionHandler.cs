using System;
using System.Xml;
using XSerializer;

namespace Rock.Configuration
{
    /// <summary>
    /// A section handler that creates an instance of a type based
    /// on an attribute named "type" defined in a section element 
    /// from a *.config file, using the XSerializer serialization
    /// library to deserialize the element.
    /// </summary>
    public class XSerializerSectionHandler : XmlSerializerSectionHandler
    {
        /// <summary>
        /// Deserialize the configuration object that will be returned by the
        /// <see cref="XmlSerializerSectionHandler.Create"/> method.
        /// </summary>
        /// <param name="section">An xml node that represents configuration object in the *.config file.</param>
        /// <param name="configType">The type of the object to be returned by the <see cref="Create"/> method.</param>
        /// <returns>The deserialized object.</returns>
        protected override object Deserialize(XmlNode section, Type configType)
        {
            var serializer = XmlSerializer.Create(configType, o => o.SetRootElementName(section.Name));
            return serializer.Deserialize(section.OuterXml);
        }
    }

    /// <summary>
    /// A section handler that creates an instance of the 
    /// <typeparamref name="TConfiguration"/> type, using the
    /// XSerializer serialization library to deserialize the element.
    /// </summary>
    public class XSerializerSectionHandler<TConfiguration> : XSerializerSectionHandler
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
