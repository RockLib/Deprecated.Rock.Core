using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Rock.IO;

namespace Rock.Serialization
{
    public class XmlSerializerSerializer : ISerializer
    {
        private readonly Encoding _encoding;

        public XmlSerializerSerializer()
            : this(Encoding.UTF8)
        {
        }

        public XmlSerializerSerializer(Encoding encoding)
        {
            _encoding = encoding ?? Encoding.UTF8;
        }

        public void SerializeToStream(Stream stream, object item, Type type)
        {
            type = CheckType(type, item);

            var serializer = new XmlSerializer(type);
            serializer.Serialize(stream, item);
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(stream);
        }

        public string SerializeToString(object item, Type type)
        {
            type = CheckType(type, item);

            var sb = new StringBuilder();

            using (var writer = new EncodedStringWriter(sb, _encoding))
            {
                var serializer = new XmlSerializer(type);
                serializer.Serialize(writer, item);
            }

            return sb.ToString();
        }

        public object DeserializeFromString(string data, Type type)
        {
            using (var reader = new StringReader(data))
            {
                var serializer = new XmlSerializer(type);
                return serializer.Deserialize(reader);
            }
        }
        /// <summary>
        /// If <paramref name="type"/> is abstract, return item.GetType().
        /// If <paramref name="type"/> is not abstract, return it.
        /// </summary>
        /// <remarks>
        /// This check allows us to handle an abstract type during
        /// serialization. There's still nothing that can be done when
        /// deserializing.
        /// </remarks>
        private static Type CheckType(Type type, object item)
        {
            return !type.IsAbstract ? type : item.GetType();
        }
    }
}