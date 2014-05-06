using System.IO;

namespace Rock.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(TextReader reader);
        void Serialize<T>(TextWriter writer, T value);
    }
}