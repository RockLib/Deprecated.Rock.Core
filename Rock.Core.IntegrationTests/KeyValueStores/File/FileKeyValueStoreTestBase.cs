using System;
using System.IO;
using NUnit.Framework;
using Rock.KeyValueStores;
using Rock.Serialization;

namespace Rock.Core.IntegrationTests.KeyValueStores
{
    public abstract class FileKeyValueStoreTestBase
    {
        protected static readonly string KeyValueStorePath = Path.Combine(Environment.GetEnvironmentVariable("Temp"), "Rock", "FileKeyValueStoreTest");
        protected static readonly ISerializer Serializer = new NewtonsoftJsonSerializer();
        private static readonly DirectoryInfo _directoryInfo = new DirectoryInfo(KeyValueStorePath);

        private Lazy<FileKeyValueStore> _keyValueStore;

        [SetUp]
        public void Setup()
        {
            Delete();
            _keyValueStore = new Lazy<FileKeyValueStore>(() => new FileKeyValueStore(Serializer, _directoryInfo));
        }

        [TearDown]
        public void Teardown()
        {
            Delete();
        }

        protected FileKeyValueStore KeyValueStore
        {
            get { return _keyValueStore.Value; }
        }


        private static void Delete()
        {
            if (Directory.Exists(KeyValueStorePath))
            {
                Directory.Delete(KeyValueStorePath, true);
            }
        }
    }
}