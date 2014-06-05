using System;
using Rock.Serialization;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ISerializer> _binarySerializer = new DefaultHelper<ISerializer>(() => new BinaryFormatterSerializer());

        public static ISerializer BinarySerializer
        {
            get { return _binarySerializer.Current; }
        }

        public static ISerializer DefaultBinarySerializer
        {
            get { return _binarySerializer.DefaultInstance; }
        }

        public static void SetBinarySerializer(Func<ISerializer> getBinarySerializerInstance)
        {
            _binarySerializer.SetCurrent(getBinarySerializerInstance);
        }

        public static void RestoreDefaultBinarySerializer()
        {
            _binarySerializer.RestoreDefault();
        }
    }
}