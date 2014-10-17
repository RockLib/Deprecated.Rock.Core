using System;
using System.IO;
using XSerializer;

namespace Rock.Serialization
{
    public class XSerializerSerializer : ISerializer
    {
        private readonly Func<XmlSerializationOptions> _createOptions; 

        public XSerializerSerializer()
            : this(new XSerializerSerializerConfiguration())
        {
        }

        public XSerializerSerializer(IXSerializerSerializerConfiguration configuration)
        {
            var namespaces = configuration.Namespaces;
            var encoding = configuration.Encoding;
            var defaultNamespace = configuration.DefaultNamespace;
            var indent = configuration.Indent;
            var alwaysEmitTypes = configuration.AlwaysEmitTypes;
            var redact = configuration.Redact;
            var treatEmptyElementAsString = configuration.TreatEmptyElementAsString;
            var emitNil = configuration.EmitNil;

            _createOptions = () =>
                new XmlSerializationOptions(
                    namespaces,
                    encoding,
                    defaultNamespace,
                    indent,
                    null, // rootElementName is not appropriate
                    alwaysEmitTypes,
                    redact,
                    treatEmptyElementAsString,
                    emitNil);
        }

        public void SerializeToStream(Stream stream, object item, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions());
            serializer.Serialize(stream, item);
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions());
            return serializer.Deserialize(stream);
        }

        public string SerializeToString(object item, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions());
            return serializer.Serialize(item);
        }

        public object DeserializeFromString(string data, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions());
            return serializer.Deserialize(data);
        }
    }
}
