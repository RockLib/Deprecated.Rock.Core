using System.IO;
using System.Text;
using Rock.Defaults.Implementation;

namespace Rock.Serialization
{
    public static class ToJsonExtension
    {
        public static string ToJson<T>(this T item)
        {
            using (var stream = new MemoryStream())
            {
                Default.JsonSerializer.Serialize(stream, item);
                stream.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
