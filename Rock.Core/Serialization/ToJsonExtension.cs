using System.IO;
using System.Text;
using Rock.Defaults.Implementation;

namespace Rock.Serialization
{
    public static class ToJsonExtension
    {
        public static string ToJson<T>(this T item)
        {
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                Default.JsonSerializer.Serialize(writer, item);
            }

            return sb.ToString();
        }
    }
}
