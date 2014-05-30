using System;
using System.IO;
using System.Text;
using XSerializer;

namespace Rock.Serialization
{
    public class XSerializerSerializer : ISerializer
    {
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            var serializer = XmlSerializer.Create(type);
            serializer.Serialize(stream, item);
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = XmlSerializer.Create(type);
            return serializer.Deserialize(stream);
        }

        public string SerializeToString(object item, Type type, Encoding encoding = null)
        {
            var serializer = XmlSerializer.Create(type, options => options.WithEncoding(encoding ?? Encoding.UTF8));
            return serializer.Serialize(item);
        }

        public object DeserializeFromString(string data, Type type, Encoding encoding = null)
        {
            var serializer = XmlSerializer.Create(type, options => options.WithEncoding(encoding ?? Encoding.UTF8));
            return serializer.Deserialize(data);
        }
    }
}
