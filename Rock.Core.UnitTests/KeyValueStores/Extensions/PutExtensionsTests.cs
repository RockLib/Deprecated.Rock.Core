using Rock.KeyValueStores;

namespace Rock.Core.UnitTests.KeyValueStores.Extensions
{
    public class PutExtensionsTests : KeyValueStoreExtensionsTestsBase
    {
        public class ThePutMethodThatExtendsIKeyValueStore : PutExtensionsTests
        {
            public void GetsABucketByNameThenCallsThePutExtensionMethodOnTheBucket()
            {
                MockKeyValueStore.Object.Put("my_bucket", "my_key", 123);

                VerifyPut("my_bucket", "my_key", 123);
            }
        }

        public class ThePutMethodThatExtendsIBucket : PutExtensionsTests
        {
            public void GetsABucketItemByKeyThenCallsThePutMethodOnTheBucketItem()
            {
                MockBucket.Object.Put("my_key", 123);

                VerifyPut("my_key", 123);
            }
        }
    }
}