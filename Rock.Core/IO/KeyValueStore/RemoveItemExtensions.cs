namespace Rock.IO
{
    public static class RemoveItemExtensions
    {
        public static void RemoveItem(this IKeyValueStore keyValueStore, string bucketName, string key)
        {
            var bucket = keyValueStore.GetOrAddBucket(bucketName);
            bucket.RemoveItem(key);
        }

        public static void RemoveItem(this IBucket bucket, string key)
        {
            var item = bucket.GetItem(key);
            item.Delete();
        }
    }
}
