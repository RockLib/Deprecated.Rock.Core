using System;
using Rock.Serialization;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<ISerializer> _defaultJsonSerializer = new Default<ISerializer>(() => new NewtonsoftJsonSerializer());

        public static ISerializer JsonSerializer
        {
            get { return _defaultJsonSerializer.Current; }
        }

        public static void SetJsonSerializer(Func<ISerializer> getJsonSerializerInstance)
        {
            _defaultJsonSerializer.SetCurrent(getJsonSerializerInstance);
        }
    }
}