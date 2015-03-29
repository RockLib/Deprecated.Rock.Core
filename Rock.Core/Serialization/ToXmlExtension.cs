using System;

namespace Rock.Serialization
{
    public static class ToXmlExtension
    {
        public static string ToXml(this object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return DefaultXmlSerializer.Current.SerializeToString(item, item.GetType());
        }
    }
}