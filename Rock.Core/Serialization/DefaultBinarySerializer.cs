using Rock.Immutable;

namespace Rock.Serialization
{
    public static class DefaultBinarySerializer
    {
        private static readonly Semimutable<ISerializer> _binarySerializer = new Semimutable<ISerializer>(GetDefault);

        public static ISerializer Current
        {
            get { return _binarySerializer.Value; }
        }

        public static void SetCurrent(ISerializer binarySerializer)
        {
            _binarySerializer.Value = binarySerializer;
        }

        private static ISerializer GetDefault()
        {
            return new BinaryFormatterSerializer();
        }
    }
}
