using System;
using System.IO;
using XSerializer;

namespace Rock.Serialization
{
    public class XSerializerSerializer : ISerializer
    {
        private readonly Func<Type, XmlSerializationOptions> _createOptions; 

        public XSerializerSerializer()
            : this(new XSerializerSerializerConfiguration())
        {
        }

        public XSerializerSerializer(IXSerializerSerializerConfiguration configuration)
        {
            var c = configuration;

            _createOptions =
                type =>
                {
                    string rootElementName;

                    return new XmlSerializationOptions(
                        c.Namespaces,
                        c.Encoding,
                        c.DefaultNamespace,
                        c.Indent,
                        c.RootElementNameMap == null
                            ? null
                            : c.RootElementNameMap.TryGetValue(type, out rootElementName)
                                ? rootElementName
                                : null,
                        c.AlwaysEmitTypes,
                        c.Redact,
                        c.TreatEmptyElementAsString,
                        c.EmitNil);
                };
        }

        public void SerializeToStream(Stream stream, object item, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            serializer.Serialize(stream, item);
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            return serializer.Deserialize(stream);
        }

        public string SerializeToString(object item, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            return serializer.Serialize(item);
        }

        public object DeserializeFromString(string data, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            return serializer.Deserialize(data);
        }
    }
}
