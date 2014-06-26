using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Rock.IO;

namespace Rock.Serialization
{
    public class XmlSerializerSerializer : ISerializer
    {
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            var serializer = new XmlSerializer(type);
            serializer.Serialize(stream, item);
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(stream);
        }

        public string SerializeToString(object item, Type type, Encoding encoding)
        {
            var sb = new StringBuilder();

            using (var writer = new EncodedStringWriter(sb, encoding))
            {
                var serializer = new XmlSerializer(type);
                serializer.Serialize(writer, item);
            }

            return sb.ToString();
        }

        public object DeserializeFromString(string data, Type type, Encoding encoding)
        {
            using (var reader = new StringReader(data))
            {
                var serializer = new XmlSerializer(type);
                return serializer.Deserialize(reader);
            }
        }
    }
}