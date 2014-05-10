using System;
using System.IO;
using Newtonsoft.Json;

namespace Rock.Serialization
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _jsonSerializer = JsonSerializer.CreateDefault();

        public void Serialize(Stream stream, object value, Type type)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    _jsonSerializer.Serialize(jsonWriter, value, type);
                }
            }
        }

        public object Deserialize(Stream stream, Type type)
        {
            using (var streamReader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    return _jsonSerializer.Deserialize(jsonReader, type);
                }
            }
        }
    }
}