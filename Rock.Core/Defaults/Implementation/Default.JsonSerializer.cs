using System;
using Rock.Serialization;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ISerializer> _jsonSerializer = new DefaultHelper<ISerializer>(() => new DataContractJsonSerializerSerializer());

        public static ISerializer JsonSerializer
        {
            get { return _jsonSerializer.Current; }
        }

        public static ISerializer DefaultJsonSerializer
        {
            get { return _jsonSerializer.DefaultInstance; }
        }

        public static void SetJsonSerializer(Func<ISerializer> getJsonSerializerInstance)
        {
            _jsonSerializer.SetCurrent(getJsonSerializerInstance);
        }

        public static void RestoreDefaultJsonSerializer()
        {
            _jsonSerializer.RestoreDefault();
        }
    }
}