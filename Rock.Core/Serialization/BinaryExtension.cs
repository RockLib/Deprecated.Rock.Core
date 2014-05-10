using Rock.Defaults.Implementation;

namespace Rock.Serialization
{
    public static class BinaryExtension
    {
        public static byte[] ToBinary(this object item)
        {
            return Default.BinarySerializer.Serialize(item);
        }
    
        public static T To<T>(this byte[] data)
        {
            return Default.BinarySerializer.Deserialize<T>(data);
        }
    }
}
