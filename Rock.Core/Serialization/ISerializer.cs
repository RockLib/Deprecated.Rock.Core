using System;
using System.IO;

namespace Rock.Serialization
{
    public interface ISerializer
    {
        void SerializeToStream(Stream stream, object item, Type type);
        object DeserializeFromStream(Stream stream, Type type);

        string SerializeToString(object item, Type type);
        object DeserializeFromString(string data, Type type);
    }
}