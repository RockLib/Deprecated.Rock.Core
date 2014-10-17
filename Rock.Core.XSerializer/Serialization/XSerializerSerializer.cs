using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            var cache = new ConcurrentDictionary<Type, XmlSerializationOptions>();

            _createOptions =
                type =>
                    cache.GetOrAdd(
                        type,
                        t =>
                        {
                            string rootElementName;

                            return new XmlSerializationOptions(
                                configuration.Namespaces,
                                configuration.Encoding,
                                configuration.DefaultNamespace,
                                configuration.Indent,
                                configuration.RootElementNameMap.TryGetValue(t, out rootElementName)
                                    ? rootElementName
                                    : null,
                                configuration.AlwaysEmitTypes,
                                configuration.Redact,
                                configuration.TreatEmptyElementAsString,
                                configuration.EmitNil);
                        });
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
