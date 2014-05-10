using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rock.Serialization
{
    public class BinaryFormatterSerializer : ISerializer
    {
        public void Serialize(Stream stream, object item, Type type)
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, item);
        }

        public object Deserialize(Stream stream, Type type)
        {
            var binaryFormatter = new BinaryFormatter();
            return binaryFormatter.Deserialize(stream);
        }
    }
}
