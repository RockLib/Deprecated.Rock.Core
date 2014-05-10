namespace Rock.KeyValueStores
{
    public static class TryGetValueExtensions
    {
        public static bool TryGetValue<T>(this IKeyValueStore keyValueStore, string bucketName, string key, out T value)
        {
            var item = keyValueStore.GetItem(bucketName, key);
            return item.TryGetValue(out value);
        }
    }
}
