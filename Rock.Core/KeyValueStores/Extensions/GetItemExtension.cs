namespace Rock.KeyValueStores
{
    public static class GetItemExtension
    {
        public static IBucketItem GetItem(this IKeyValueStore keyValueStore, string bucketName, string key)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            return bucket.GetItem(key);
        }
    }
}
