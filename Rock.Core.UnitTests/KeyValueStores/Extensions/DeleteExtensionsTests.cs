using NUnit.Framework;
using Rock.Core.UnitTests.KeyValueStores.Extensions;
using Rock.KeyValueStores;

// ReSharper disable once CheckNamespace
namespace DeleteExtensionsTests
{
    public class TheDeleteMethodThatExtendsIKeyValueStore : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketItemByBucketNameAndKeyThenCallsTheDeleteMethodOnTheBucketItem()
        {
            MockKeyValueStore.Object.Delete("my_bucket", "my_key");

            VerifyDelete("my_bucket", "my_key");
        }
    }

    public class TheDeleteMethodThatExtendsIBucket : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketItemByKeyThenCallsTheDeleteMethodOnTheBucketItem()
        {
            MockBucket.Object.Delete("my_key");

            VerifyDelete("my_key");
        }
    }
}
