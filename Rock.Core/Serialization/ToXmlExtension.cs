using Rock.Defaults.Implementation;

namespace Rock.Serialization
{
    public static class ToXmlExtension
    {
        public static string ToXml<T>(this T item)
        {
            return Default.XmlSerializer.SerializeToString(item);
        }
    }
}