using NUnit.Framework;
using Rock.Core.UnitTests.KeyValueStores.Extensions;
using Rock.KeyValueStores;

// ReSharper disable once CheckNamespace
namespace GetItemExtensionTests
{
    public class TheGetItemExtensionMethod : KeyValueStoreExtensionsTestsBase
    {
        [Test]
        public void GetsABucketByNameThenCallsGetItemOnTheBucket()
        {
            MockKeyValueStore.Object.GetItem("my_bucket", "my_key");

            VerifyGetItem("my_bucket", "my_key");
        }
    }
}
