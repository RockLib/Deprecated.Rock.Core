using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Rock.Serialization
{
    public class DataContractJsonSerializerSerializer : ISerializer
    {
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            var serializer = new DataContractJsonSerializer(type);
            serializer.WriteObject(stream, item);
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = new DataContractJsonSerializer(type);
            return serializer.ReadObject(stream);
        }

        public string SerializeToString(object item, Type type, Encoding encoding)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(type);
                serializer.WriteObject(stream, item);
                stream.Flush();
                return (encoding ?? Encoding.UTF8).GetString(stream.ToArray());
            }
        }

        public object DeserializeFromString(string data, Type type, Encoding encoding)
        {
            using (var stream = new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(data)))
            {
                var serializer = new DataContractJsonSerializer(type);
                return serializer.ReadObject(stream);
            }
        }
    }
}