using System;

namespace Rock.Serialization
{
    public interface IBinarySerializer
    {
        byte[] Serialize(object item);
        object Deserialize(byte[] data);
    }
}
