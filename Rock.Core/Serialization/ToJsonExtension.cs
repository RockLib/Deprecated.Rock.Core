namespace Rock.Serialization
{
    public static class ToJsonExtension
    {
        public static string ToJson(this object item)
        {
            return Default.JsonSerializer.Serialize(item);
        }
    }
}
