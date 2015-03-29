using Rock.Immutable;

namespace Rock.Serialization
{
    public static class DefaultJsonSerializer
    {
        private static readonly Semimutable<ISerializer> _jsonSerializer = new Semimutable<ISerializer>(GetDefault);

        public static ISerializer Current
        {
            get { return _jsonSerializer.Value; }
        }

        public static void SetCurrent(ISerializer jsonSerializer)
        {
            _jsonSerializer.Value = jsonSerializer;
        }

        private static ISerializer GetDefault()
        {
            return new DataContractJsonSerializerSerializer();
        }
    }
}