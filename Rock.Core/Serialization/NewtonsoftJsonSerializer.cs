using Newtonsoft.Json;

namespace Rock.Serialization
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize(object item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}