namespace Rock.IO
{
    public static class GetItemExtensions
    {
        public static IBucketItem GetItem(this IKeyValueStore keyValueStore, string bucketName, string key)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            return bucket.GetItem(key);
        }

        public static IExpirableBucketItem GetItem(this IExpirableKeyValueStore keyValueStore, string bucketName, string key)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            return bucket.GetItem(key);
        }
    }
}
