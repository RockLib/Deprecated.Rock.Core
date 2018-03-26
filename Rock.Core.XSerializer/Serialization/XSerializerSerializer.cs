using System;
using System.IO;
using Rock.StaticDependencyInjection;
using XSerializer;

namespace Rock.Serialization
{
    /// <summary>
    /// Defines methods to serialize xml documents using the XSerializer library.
    /// </summary>
    [Export(Name = "XmlSerializer")]
    public class XSerializerSerializer : ISerializer
    {
        private readonly Func<Type, XmlSerializationOptions> _createOptions; 

        /// <summary>
        /// Initializes a new instance of the <see cref="XSerializerSerializer"/> class.
        /// </summary>
        public XSerializerSerializer()
            : this(new XSerializerSerializerConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XSerializerSerializer"/> class
        /// with the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration used to initialize objects from the XSerializer library.</param>
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
						false,
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

        /// <summary>
        /// Serialize the <paramref name="item"/> object, as type <paramref name="type"/>
        /// to the <paramref name="stream"/> stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type to serialize the object as.</param>
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            serializer.Serialize(stream, item);
        }

        /// <summary>
        /// Deserialize from the <paramref name="stream"/> stream as type
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="type">The type to deserialize as.</param>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            return serializer.Deserialize(stream);
        }

        /// <summary>
        /// Serialize the <paramref name="item"/> object, as type <paramref name="type"/>
        /// to a string.
        /// </summary>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type to serialize the object as.</param>
        /// <returns>A string representing the <paramref name="item"/> object.</returns>
        public string SerializeToString(object item, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            return serializer.Serialize(item);
        }

        /// <summary>
        /// Deserialize from the string as type <paramref name="type"/>.
        /// </summary>
        /// <param name="data">The string containing a representation of an object of type <paramref name="type"/>.</param>
        /// <param name="type">The type to deserialize as.</param>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        public object DeserializeFromString(string data, Type type)
        {
            var serializer = XmlSerializer.Create(type, _createOptions(type));
            return serializer.Deserialize(data);
        }
    }
}
