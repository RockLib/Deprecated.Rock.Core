using Rock.Defaults.Implementation;

namespace Rock.Serialization
{
    public static class ToJsonExtension
    {
        public static string ToJson<T>(this T item)
        {
            return Default.JsonSerializer.SerializeToString(item);
        }
    }
}
