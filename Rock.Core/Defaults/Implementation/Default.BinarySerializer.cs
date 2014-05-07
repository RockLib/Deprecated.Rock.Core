using System;
using Rock.Serialization;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IBinarySerializer> _binarySerializer = new DefaultHelper<IBinarySerializer>(() => new BclBinarySerializer());

        public static IBinarySerializer BinarySerializer
        {
            get { return _binarySerializer.Current; }
        }

        public static void SetBinarySerializer(Func<IBinarySerializer> getBinarySerializerInstance)
        {
            _binarySerializer.SetCurrent(getBinarySerializerInstance);
        }
    }
}