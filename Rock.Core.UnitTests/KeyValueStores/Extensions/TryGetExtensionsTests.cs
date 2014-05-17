using NUnit.Framework;
using Rock.KeyValueStores;

namespace Rock.Core.UnitTests.KeyValueStores.Extensions
{
    public class TryGetExtensionsTests : KeyValueStoreExtensionsTestsBase
    {
        public class TheGetMethodThatExtendsIKeyValueStore : TryGetExtensionsTests
        {
            [Test]
            public void GetsABucketItemByBucketNameAndKeyThenCallsTheTryGetMethodOnTheBucketItem()
            {
                SetupTryGet<int>();

                int dummy;
                MockKeyValueStore.Object.TryGet("my_bucket", "my_key", out dummy);

                VerifyTryGet<int>("my_bucket", "my_key");
            }

            [Test]
            public void ReturnsTrueWhenTheTryGetMethodOfTheBucketItemReturnsTrue()
            {
                SetupTryGet<int>();

                int dummy;
                var success = MockKeyValueStore.Object.TryGet("my_bucket", "my_key", out dummy);

                Assert.That(success, Is.True);
            }

            [Test]
            public void ReturnsFalseWhenTheTryGetMethodOfTheBucketItemReturnsFalse()
            {

                int dummy;
                var success = MockKeyValueStore.Object.TryGet("my_bucket", "my_key", out dummy);

                Assert.That(success, Is.False);
            }

            [Test]
            public void HasTheSameOutParameterOfTheTryGetMethodOfTheBucketItem()
            {
                SetupTryGet(123);

                int value;
                MockKeyValueStore.Object.TryGet("my_bucket", "my_key", out value);

                Assert.That(value, Is.EqualTo(123));
            }
        }

        public class TheGetMethodThatExtendsIBucket : TryGetExtensionsTests
        {
            [Test]
            public void GetsABucketItemByKeyThenCallsTheTryGetMethodOnTheBucketItem()
            {
                SetupTryGet<int>();

                int dummy;
                MockBucket.Object.TryGet("my_key", out dummy);

                VerifyTryGet<int>("my_key");
            }

            [Test]
            public void ReturnsTrueWhenTheTryGetMethodOfTheBucketItemReturnsTrue()
            {
                SetupTryGet<int>();

                int dummy;
                var success = MockBucket.Object.TryGet("my_key", out dummy);

                Assert.That(success, Is.True);
            }

            [Test]
            public void ReturnsFalseWhenTheTryGetMethodOfTheBucketItemReturnsFalse()
            {

                int dummy;
                var success = MockBucket.Object.TryGet("my_key", out dummy);

                Assert.That(success, Is.False);
            }

            [Test]
            public void HasTheSameOutParameterOfTheTryGetMethodOfTheBucketItem()
            {
                SetupTryGet(123);

                int value;
                MockBucket.Object.TryGet("my_key", out value);

                Assert.That(value, Is.EqualTo(123));
            }
        }
    }
}
