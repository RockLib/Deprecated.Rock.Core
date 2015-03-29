using Rock.Immutable;

namespace Rock.Serialization
{
    public static class DefaultXmlSerializer
    {
        private static readonly Semimutable<ISerializer> _xmlSerializer = new Semimutable<ISerializer>(GetDefault);

        public static ISerializer Current
        {
            get { return _xmlSerializer.Value; }
        }

        public static void SetCurrent(ISerializer xmlSerializer)
        {
            _xmlSerializer.Value = xmlSerializer;
        }

        private static ISerializer GetDefault()
        {
            return new XmlSerializerSerializer();
        }
    }
}