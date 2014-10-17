using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Rock.Configuration
{
    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return Deserialize(section, GetConfigType(section));
        }

        protected virtual object Deserialize(XmlNode section, Type configType)
        {
            var serializer = new XmlSerializer(configType, new XmlRootAttribute(section.Name));

            using (var reader = new StringReader(section.OuterXml))
            {
                return serializer.Deserialize(reader);
            }
        }

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

    public class XmlSerializerSectionHandler<TConfiguration> : XmlSerializerSectionHandler
    {
        protected override Type GetConfigType(XmlNode section)
        {
            return typeof(TConfiguration);
        }
    }
}
