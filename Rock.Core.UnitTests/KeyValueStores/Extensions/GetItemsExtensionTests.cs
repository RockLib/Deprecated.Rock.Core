using NUnit.Framework;
using Rock.KeyValueStores;

namespace Rock.Core.UnitTests.KeyValueStores.Extensions
{
    public class GetItemsExtensionTests : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketByNameThenCallsGetItemsOnTheBucket()
        {
            MockKeyValueStore.Object.GetItems("my_bucket");

            VerifyGetItems("my_bucket");
        }
    }
}