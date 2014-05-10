using System;
using System.IO;

namespace Rock.Serialization
{
    public interface ISerializer
    {
        void Serialize(Stream stream, object item, Type type);
        object Deserialize(Stream stream, Type type);
    }
}