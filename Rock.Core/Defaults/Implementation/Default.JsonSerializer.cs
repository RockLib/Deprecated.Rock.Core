using System;
using Rock.Serialization;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ISerializer> _jsonSerializer = new DefaultHelper<ISerializer>(() => new NewtonsoftJsonSerializer());

        public static ISerializer JsonSerializer
        {
            get { return _jsonSerializer.Current; }
        }

        public static void SetJsonSerializer(Func<ISerializer> getJsonSerializerInstance)
        {
            _jsonSerializer.SetCurrent(getJsonSerializerInstance);
        }
    }
}