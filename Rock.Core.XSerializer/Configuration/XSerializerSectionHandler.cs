using System;
using System.Xml;
using XSerializer;

namespace Rock.Configuration
{
    public class XSerializerSectionHandler : XmlSerializerSectionHandler
    {
        protected override object Deserialize(XmlNode section, Type configType)
        {
            var serializer = XmlSerializer.Create(configType, o => o.SetRootElementName(section.Name));
            return serializer.Deserialize(section.OuterXml);
        }
    }

    public class XSerializerSectionHandler<TConfiguration> : XSerializerSectionHandler
    {
        protected override Type GetConfigType(XmlNode section)
        {
            return typeof(TConfiguration);
        }
    }
}
