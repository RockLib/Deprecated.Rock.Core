using System.Collections.Generic;

namespace Rock.IO
{
    public static class KeyValueStorageExtensions
    {
        public static IEnumerable<IBucketItem> GetItems(this IKeyValueStorage keyValueStorage, string bucketName)
        {
            var bucket = keyValueStorage.GetBucket(bucketName);
            return bucket.GetItems();
        }

        public static IBucketItem GetItem(this IKeyValueStorage keyValueStorage, string bucketName, string key)
        {
            var bucket = keyValueStorage.GetBucket(bucketName);
            return bucket.GetItem(key);
        }

        public static T GetValue<T>(this IKeyValueStorage keyValueStorage, string bucketName, string key)
        {
            var item = keyValueStorage.GetItem(bucketName, key);
            return item.GetValue<T>();
        }

        public static void AddItem<T>(this IKeyValueStorage keyValueStorage, string bucketName, string key, T value)
        {
            var bucket = keyValueStorage.GetBucket(bucketName);
            bucket.AddItem<T>(key, value);
        }

        public static void RemoveItem(this IKeyValueStorage keyValueStorage, string bucketName, string key)
        {
            var bucket = keyValueStorage.GetBucket(bucketName);
            bucket.RemoveItem(key);
        }
    }
}