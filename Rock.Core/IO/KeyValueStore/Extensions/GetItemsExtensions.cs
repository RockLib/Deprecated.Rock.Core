using System.Collections.Generic;

namespace Rock.IO
{
    public static class GetItemsExtensions
    {
        public static IEnumerable<IBucketItem> GetItems(this IKeyValueStore keyValueStore, string bucketName)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            return bucket.GetItems();
        }

        public static IEnumerable<IExpirableBucketItem> GetItems(this IExpirableKeyValueStore keyValueStore, string bucketName)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            return bucket.GetItems();
        }
    }
}
