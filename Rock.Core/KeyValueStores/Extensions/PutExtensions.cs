namespace Rock.KeyValueStores
{
    public static class PutExtensions
    {
        public static void Put<T>(this IKeyValueStore keyValueStore, string bucketName, string key, T value)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            bucket.Put(key, value);
        }

        public static void Put<T>(this IBucket bucket, string key, T value)
        {
            var item = bucket.GetItem(key);
            item.Put(value);
        }
    }
}
