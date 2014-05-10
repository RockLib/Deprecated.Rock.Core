using System.Collections.Generic;

namespace Rock.KeyValueStores
{
    public static class GetItemsExtension
    {
        public static IEnumerable<IBucketItem> GetItems(this IKeyValueStore keyValueStore, string bucketName)
        {
            var bucket = keyValueStore.GetBucket(bucketName);
            return bucket.GetItems();
        }
    }
}
