using System.IO;
using Newtonsoft.Json;

namespace Rock.Serialization
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _jsonSerializer = JsonSerializer.CreateDefault();

        public T Deserialize<T>(TextReader reader)
        {
            using (var jsonReader = new JsonTextReader(reader))
            {
                return _jsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        public void Serialize<T>(TextWriter writer, T value)
        {
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                _jsonSerializer.Serialize(jsonWriter, value, typeof(T));
            }
        }
    }
}