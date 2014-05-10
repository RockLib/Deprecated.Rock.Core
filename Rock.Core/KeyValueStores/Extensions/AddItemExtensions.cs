namespace Rock.KeyValueStores
{
    public static class AddItemExtensions
    {
        public static void AddItem<T>(this IKeyValueStore keyValueStore, string bucketName, string key, T value)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            bucket.AddItem(key, value);
        }

        public static void AddItem<T>(this IBucket bucket, string key, T value)
        {
            var item = bucket.GetItem(key);
            item.SetValue(value);
        }
    }
}
