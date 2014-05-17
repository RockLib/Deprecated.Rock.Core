namespace Rock.KeyValueStores
{
    public static class TryGetExtensions
    {
        public static bool TryGet<T>(this IKeyValueStore keyValueStore, string bucketName, string key, out T value)
        {
            var item = keyValueStore.GetItem(bucketName, key);
            return item.TryGet(out value);
        }

        public static bool TryGet<T>(this IBucket bucket, string key, out T value)
        {
            var item = bucket.GetItem(key);
            return item.TryGet(out value);
        }
    }
}
