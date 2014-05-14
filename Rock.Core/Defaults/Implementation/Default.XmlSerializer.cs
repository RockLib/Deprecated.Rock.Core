using System;
using Rock.Serialization;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<ISerializer> _xmlSerializer = new DefaultHelper<ISerializer>(() => new XSerializerSerializer());

        public static ISerializer XmlSerializer
        {
            get { return _xmlSerializer.Current; }
        }

        public static ISerializer DefaultXmlSerializer
        {
            get { return _xmlSerializer.DefaultInstance; }
        }

        public static void SetXmlSerializer(Func<ISerializer> getXmlSerializerInstance)
        {
            _xmlSerializer.SetCurrent(getXmlSerializerInstance);
        }
    }
}
