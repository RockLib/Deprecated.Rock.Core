using System;
using System.IO;
using XSerializer;

namespace Rock.Serialization
{
    public class XSerializerSerializer : ISerializer
    {
        public void Serialize(Stream stream, object item, Type type)
        {
            var serializer = XmlSerializer.Create(type);
            serializer.Serialize(stream, item);
        }

        public object Deserialize(Stream stream, Type type)
        {
            var serializer = XmlSerializer.Create(type);
            return serializer.Deserialize(stream);
        }
    }
}
