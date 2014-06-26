using System;
using System.IO;
using System.Text;

namespace Rock.Serialization
{
    public interface ISerializer
    {
        void SerializeToStream(Stream stream, object item, Type type);
        object DeserializeFromStream(Stream stream, Type type);

        string SerializeToString(object item, Type type, Encoding encoding = null);
        object DeserializeFromString(string data, Type type);
    }
}