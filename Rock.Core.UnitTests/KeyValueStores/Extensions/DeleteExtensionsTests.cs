using NUnit.Framework;
using Rock.KeyValueStores;

namespace Rock.Core.UnitTests.KeyValueStores.Extensions
{
    public class DeleteExtensionsTests : KeyValueStoreExtensionsTestsBase
    {
        public class TheDeleteMethodThatExtendsIKeyValueStore : DeleteExtensionsTests
        {
            [Test]
            public void GetsABucketItemByBucketNameAndKeyThenCallsTheDeleteMethodOnTheBucketItem()
            {
                MockKeyValueStore.Object.Delete("my_bucket", "my_key");

                VerifyDelete("my_bucket", "my_key");
            }
        }

        public class TheDeleteMethodThatExtendsIBucket : DeleteExtensionsTests
        {
            [Test]
            public void GetsABucketItemByKeyThenCallsTheDeleteMethodOnTheBucketItem()
            {
                MockBucket.Object.Delete("my_key");

                VerifyDelete("my_key");
            }
        }
    }
}
