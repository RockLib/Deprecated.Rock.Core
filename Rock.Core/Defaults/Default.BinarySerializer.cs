using System;
using Rock.Serialization;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IBinarySerializer> _defaultBinarySerializer = new Default<IBinarySerializer>(() => new BclBinarySerializer());

        public static IBinarySerializer BinarySerializer
        {
            get { return _defaultBinarySerializer.Current; }
        }

        public static void SetBinarySerializer(Func<IBinarySerializer> getBinarySerializerInstance)
        {
            _defaultBinarySerializer.SetCurrent(getBinarySerializerInstance);
        }
    }
}