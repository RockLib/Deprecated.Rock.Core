using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Rock.Serialization
{
    public class DataContractJsonSerializerSerializer : ISerializer
    {
        private readonly Encoding _encoding;

        public DataContractJsonSerializerSerializer()
            : this(Encoding.UTF8)
        {
        }

        public DataContractJsonSerializerSerializer(Encoding encoding)
        {
            _encoding = encoding ?? Encoding.UTF8;
        }

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

        public string SerializeToString(object item, Type type)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(type);
                serializer.WriteObject(stream, item);
                stream.Flush();
                return _encoding.GetString(stream.ToArray());
            }
        }

        public object DeserializeFromString(string data, Type type)
        {
            using (var stream = new MemoryStream(_encoding.GetBytes(data)))
            {
                var serializer = new DataContractJsonSerializer(type);
                return serializer.ReadObject(stream);
            }
        }
    }
}