using NUnit.Framework;
using Rock.Core.UnitTests.KeyValueStores.Extensions;
using Rock.KeyValueStores;

// ReSharper disable once CheckNamespace
namespace GetExtensionsTests
{
    public class TheGetMethodThatExtendsIKeyValueStore : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketItemByBucketNameAndKeyThenCallsTheGetExtensionMethodOnTheBucketItem()
        {
            SetupTryGet<int>();

            MockKeyValueStore.Object.Get<int>("my_bucket", "my_key");

            VerifyGet<int>("my_bucket", "my_key");
        }

        [Test]
        public void ReturnsWhatTheTryGetMethodOfTheBucketItemAssignsToTheOutParameter()
        {
            SetupTryGet(123);

            var value = MockKeyValueStore.Object.Get<int>("my_bucket", "my_key");

            Assert.That(value, Is.EqualTo(123));
        }
    }

    public class TheGetMethodThatExtendsIBucket : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketItemByKeyThenCallsTheGetExtensionMethodOnTheBucketItem()
        {
            SetupTryGet<int>();

            MockBucket.Object.Get<int>("my_key");

            VerifyGet<int>("my_key");
        }

        [Test]
        public void ReturnsWhatTheTryGetMethodOfTheBucketItemAssignsToTheOutParameter()
        {
            SetupTryGet(123);

            var value = MockBucket.Object.Get<int>("my_key");

            Assert.That(value, Is.EqualTo(123));
        }
    }

    public class TheGetMethodThatExtendsIBucketItem : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void WhenTryGetIsSuccessfulReturnsTheValueOfTheOutParameter()
        {
            SetupTryGet(123);

            var value = MockBucketItem.Object.Get<int>();

            Assert.That(value, Is.EqualTo(123));
        }

        [Test]
        public void WhenTryGetIsUnsuccessfulThrowAnException()
        {
            Assert.That(() => MockBucketItem.Object.Get<int>(), Throws.Exception);
        }
    }
}
