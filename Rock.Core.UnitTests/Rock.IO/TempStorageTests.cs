using NUnit.Framework;
using Rock.Core.UnitTests.KeyValueStores.Extensions;
using Rock.Defaults.Implementation;
using Rock.IO;

// ReSharper disable once CheckNamespace
namespace TempStorageTests
{
    public class TempStorageTests : KeyValueStoreExtensionsTestsBase
    {
        [SetUp]
        public new void Setup()
        {
            Default.SetTempStorage(() => MockKeyValueStore.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Default.SetTempStorage(null);
        }

        public class TheGetItemsMethod : TempStorageTests
        {
            [Test]
            public void CallsTheGetItemsExtensionMethodOfDefaultTempStorage()
            {
                TempStorage.GetItems("my_bucket");

                VerifyGetItems("my_bucket");
            }
        }

        public class ThePutMethod : TempStorageTests
        {
            [Test]
            public void CallsThePutExtensionMethodOfDefaultTempStorage()
            {
                TempStorage.Put("my_bucket", "my_key", 123);

                VerifyPut("my_bucket", "my_key", 123);
            }
        }

        public class TheGetMethod : TempStorageTests
        {
            [Test]
            public void CallsTheGetExtensionMethodOfDefaultTempStorage()
            {
                SetupTryGet<int>();

                TempStorage.Get<int>("my_bucket", "my_key");

                VerifyGet<int>("my_bucket", "my_key");
            }
        }

        public class TheDeleteMethod : TempStorageTests
        {
            [Test]
            public void CallsTheDeleteExtensionMethodOfDefaultTempStorage()
            {
                TempStorage.Delete("my_bucket", "my_key");

                VerifyDelete("my_bucket", "my_key");
            }
        }
    }
}