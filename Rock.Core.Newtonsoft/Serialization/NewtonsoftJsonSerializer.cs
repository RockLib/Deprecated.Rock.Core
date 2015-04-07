using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Rock.IO;
using Rock.StaticDependencyInjection;

namespace Rock.Serialization
{
    [Export(Name="JsonSerializer")]
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _jsonSerializer;
        private readonly Encoding _encoding;

        public NewtonsoftJsonSerializer()
            : this(null, Encoding.UTF8)
        {
        }

        public NewtonsoftJsonSerializer(JsonSerializer jsonSerializer, Encoding encoding)
        {
            _encoding = encoding ?? Encoding.UTF8;
            _jsonSerializer = jsonSerializer ?? JsonSerializer.CreateDefault();
        }

        public void SerializeToStream(Stream stream, object value, Type type)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    _jsonSerializer.Serialize(jsonWriter, value, type);
                }
            }
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            using (var streamReader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    return _jsonSerializer.Deserialize(jsonReader, type);
                }
            }
        }

        public string SerializeToString(object item, Type type)
        {
            var sb = new StringBuilder();

            using (var stringWriter = new EncodedStringWriter(sb, _encoding))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    _jsonSerializer.Serialize(jsonWriter, item, type);
                }
            }

            return sb.ToString();
        }

        public object DeserializeFromString(string data, Type type)
        {
            using (var stringReader = new StringReader(data))
            {
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    return _jsonSerializer.Deserialize(jsonReader, type);
                }
            }
        }
    }
}