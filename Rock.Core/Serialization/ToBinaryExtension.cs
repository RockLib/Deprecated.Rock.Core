using System;

namespace Rock.Serialization
{
    public static class ToBinaryExtension
    {
        public static byte[] ToBinary(this object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return DefaultBinarySerializer.Current.SerializeToByteArray(item, item.GetType());
        }
    }
}
