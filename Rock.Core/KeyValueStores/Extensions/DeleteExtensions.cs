namespace Rock.KeyValueStores
{
    public static class DeleteExtensions
    {
        public static void Delete(this IKeyValueStore keyValueStore, string bucketName, string key)
        {
            var item = keyValueStore.GetItem(bucketName, key);
            item.Delete();
        }

        public static void Delete(this IBucket bucket, string key)
        {
            var item = bucket.GetItem(key);
            item.Delete();
        }
    }
}
