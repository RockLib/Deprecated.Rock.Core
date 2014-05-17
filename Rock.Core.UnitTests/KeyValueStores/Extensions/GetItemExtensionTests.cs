using NUnit.Framework;
using Rock.KeyValueStores;

namespace Rock.Core.UnitTests.KeyValueStores.Extensions
{
    public class GetItemExtensionTests : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketByNameThenCallsGetItemOnTheBucket()
        {
            MockKeyValueStore.Object.GetItem("my_bucket", "my_key");

            VerifyGetItem("my_bucket", "my_key");
        }
    }
}
