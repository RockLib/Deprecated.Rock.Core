using NUnit.Framework;
using Rock.Core.UnitTests.KeyValueStores.Extensions;
using Rock.KeyValueStores;

// ReSharper disable once CheckNamespace
namespace GetItemsExtensionTests
{
    public class TheGetItemsExtensionMethod : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketByNameThenCallsGetItemsOnTheBucket()
        {
            MockKeyValueStore.Object.GetItems("my_bucket");

            VerifyGetItems("my_bucket");
        }
    }
}