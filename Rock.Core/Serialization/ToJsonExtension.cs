using System;

namespace Rock.Serialization
{
    public static class ToJsonExtension
    {
        public static string ToJson(this object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return DefaultJsonSerializer.Current.SerializeToString(item, item.GetType());
        }
    }
}
