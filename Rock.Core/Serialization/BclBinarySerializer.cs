using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rock.Serialization
{
    /// <remarks>
    /// Bcl = Base Clas Library - the set of libraries that come with the .net framework
    /// </remarks>
    public class BclBinarySerializer : IBinarySerializer
    {
        public byte[] Serialize(object item)
        {
            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, item);
            return memoryStream.GetBuffer();
        }

        public object Deserialize(byte[] data)
        {
            var memoryStream = new MemoryStream(data);
            var binaryFormatter = new BinaryFormatter();
            memoryStream.Position = 0;
            return binaryFormatter.Deserialize(memoryStream);
        }
    }
}
