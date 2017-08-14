using System;
using System.Collections.Generic;
using System.IO;
using Rock.Immutable;
using Rock.KeyValueStores;
using Rock.Serialization;

namespace Rock.IO
{
    public static class TempStorage
    {
        private static readonly Semimutable<IKeyValueStore> _keyValueStore = new Semimutable<IKeyValueStore>(GetDefaultKeyValueStore);

        public static IKeyValueStore KeyValueStore
        {
            get { return _keyValueStore.Value; }
        }

        public static void SetKeyValueStore(IKeyValueStore keyValueStore)
        {
            _keyValueStore.Value = keyValueStore;
        }

        internal static void ResetKeyValueStore()
        {
            UnlockKeyValueStore();
            _keyValueStore.ResetValue();
        }

        internal static void UnlockKeyValueStore()
        {
            _keyValueStore.GetUnlockValueMethod().Invoke(_keyValueStore, null);
        }

        private static IKeyValueStore GetDefaultKeyValueStore()
        {
            var tempDirectory = Environment.GetEnvironmentVariable("Temp");
            var tempStorageDirectoryInfo = new DirectoryInfo(Path.Combine(tempDirectory, "Rock", ApplicationId.Current));
            return new FileKeyValueStore(DefaultJsonSerializer.Current, tempStorageDirectoryInfo);
        }

        public static IEnumerable<IBucketItem> GetItems(string bucket)
        {
            return KeyValueStore.GetItems(bucket);
        }

        public static void Put<T>(string bucket, string key, T value)
        {
            KeyValueStore.Put(bucket, key, value);
        }

        public static T Get<T>(string bucket, string key)
        {
            return KeyValueStore.Get<T>(bucket, key);
        }

        public static void Delete(string bucket, string key)
        {
            KeyValueStore.Delete(bucket, key);
        }
    }
}