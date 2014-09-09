using NUnit.Framework;
using Rock.Core.UnitTests.KeyValueStores.Extensions;
using Rock.KeyValueStores;

// ReSharper disable once CheckNamespace
namespace PutExtensionsTests
{
    public class ThePutMethodThatExtendsIKeyValueStore : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketByNameThenCallsThePutExtensionMethodOnTheBucket()
        {
            MockKeyValueStore.Object.Put("my_bucket", "my_key", 123);

            VerifyPut("my_bucket", "my_key", 123);
        }
    }

    public class ThePutMethodThatExtendsIBucket : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketItemByKeyThenCallsThePutMethodOnTheBucketItem()
        {
            MockBucket.Object.Put("my_key", 123);

            VerifyPut("my_key", 123);
        }
    }
}