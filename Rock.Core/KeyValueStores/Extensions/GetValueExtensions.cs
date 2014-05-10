using System;

namespace Rock.KeyValueStores
{
    public static class GetValueExtensions
    {
        public static T GetValue<T>(this IKeyValueStore keyValueStore, string bucketName, string key)
        {
            var item = keyValueStore.GetItem(bucketName, key);
            return item.GetValue<T>();
        }

        public static T GetValue<T>(this IBucketItem item)
        {
            T value;

            if (!item.TryGetValue(out value))
            {
                // TODO: better exeption type.
                throw new Exception(string.Format("Key not found for bucket: {0}.{1}", item.BucketName, item.Key));
            }

            return value;
        }
    }
}
