using System.Collections.Generic;
using Rock.Defaults.Implementation;
using Rock.KeyValueStores;

namespace Rock.IO
{
    public static class TempStorage
    {
        public static IEnumerable<IBucketItem> GetItems(string bucket)
        {
            return Default.TempStorage.GetItems(bucket);
        }

        public static void Write<T>(string bucket, string key, T value)
        {
            Default.TempStorage.AddItem(bucket, key, value);
        }

        public static T Get<T>(string bucket, string key)
        {
            return Default.TempStorage.GetValue<T>(bucket, key);
        }

        public static void Delete(string bucket, string key)
        {
            Default.TempStorage.GetItem(bucket, key).Delete();
        }
    }
}