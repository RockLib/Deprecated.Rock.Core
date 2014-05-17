using Moq;
using NUnit.Framework;
using Rock.KeyValueStores;

namespace Rock.Core.UnitTests.KeyValueStores.Extensions
{
    public abstract class KeyValueStoreExtensionsTestsBase
    {
        [SetUp]
        public void Setup()
        {
            MockKeyValueStore = new Mock<IKeyValueStore>();
            MockBucket = new Mock<IBucket>();
            MockBucketItem = new Mock<IBucketItem>();
            
            var buckets = new[] { MockBucket.Object };
            MockKeyValueStore.Setup(m => m.GetBucket(It.IsAny<string>())).Returns(() => MockBucket.Object);
            MockKeyValueStore.Setup(m => m.GetBuckets()).Returns(() => buckets);

            var items = new[] { MockBucketItem.Object };
            MockBucket.Setup(m => m.GetItem(It.IsAny<string>())).Returns(() => MockBucketItem.Object);
            MockBucket.Setup(m => m.GetItems()).Returns(() => items);
        }

        protected Mock<IKeyValueStore> MockKeyValueStore { get; private set; }
        protected Mock<IBucket> MockBucket { get; private set; }
        protected Mock<IBucketItem> MockBucketItem { get; private set; }

        protected void SetupTryGet<T>()
        {
            SetupTryGet(default(T));
        }

        // ReSharper disable once RedundantAssignment
        protected void SetupTryGet<T>(T value)
        {
            MockBucketItem.Setup(m => m.TryGet(out value)).Returns(true);
        }

        protected void VerifyGetItems(string bucketName)
        {
            VerifyGetBucket(bucketName);
            MockBucket.Verify(m => m.GetItems(), Times.Once);
        }

        protected void VerifyGetItem(string bucketName, string key)
        {
            VerifyGetBucket(bucketName);
            VerifyGetItem(key);
        }

        protected void VerifyPut<T>(string bucketName, string key, T value)
        {
            VerifyGetBucket(bucketName);
            VerifyPut(key, value);
        }

        protected void VerifyGet<T>(string bucketName, string key)
        {
            VerifyGetItem("my_bucket", "my_key");
            VerifyGet<T>();
        }

        protected void VerifyGet<T>(string key)
        {
            VerifyGetItem(key);
            VerifyGet<T>();
        }

        protected void VerifyGet<T>()
        {
            VerifyTryGet<T>();
        }

        protected void VerifyTryGet<T>(string bucketName, string key)
        {
            VerifyGetItem(bucketName, key);
            VerifyTryGet<T>();
        }

        protected void VerifyTryGet<T>(string key)
        {
            VerifyGetItem(key);
            VerifyTryGet<T>();
        }

        protected void VerifyTryGet<T>()
        {
            T dummy;
            MockBucketItem.Verify(m => m.TryGet(out dummy), Times.Once());
        }

        protected void VerifyPut<T>(string key, T value)
        {
            VerifyGetItem(key);
            MockBucketItem.Verify(m => m.Put(It.Is<T>(t => Equals(t, value))), Times.Once);
        }

        protected void VerifyDelete(string bucketName, string key)
        {
            VerifyGetItem(bucketName, key);
            VerifyDelete();
        }

        protected void VerifyDelete(string key)
        {
            VerifyGetItem(key);
            VerifyDelete();
        }

        private void VerifyGetBucket(string bucketName)
        {
            MockKeyValueStore.Verify(m => m.GetBucket(It.Is<string>(s => s == bucketName)), Times.Once);
        }

        private void VerifyGetItem(string key)
        {
            MockBucket.Verify(m => m.GetItem(It.Is<string>(k => k == key)), Times.Once);
        }

        private void VerifyDelete()
        {
            MockBucketItem.Verify(m => m.Delete(), Times.Once);
        }
    }
}