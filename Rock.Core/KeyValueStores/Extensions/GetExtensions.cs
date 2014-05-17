using System;

namespace Rock.KeyValueStores
{
    public static class GetExtensions
    {
        public static T Get<T>(this IKeyValueStore keyValueStore, string bucketName, string key)
        {
            var item = keyValueStore.GetItem(bucketName, key);
            return item.Get<T>();
        }

        public static T Get<T>(this IBucket bucket, string key)
        {
            var item = bucket.GetItem(key);
            return item.Get<T>();
        }

        public static T Get<T>(this IBucketItem item)
        {
            T value;

            if (!item.TryGet(out value))
            {
                // TODO: better exeption type.
                throw new Exception(string.Format("Key not found for bucket: {0}.{1}", item.BucketName, item.Key));
            }

            return value;
        }
    }
}
