using System.IO;
using NUnit.Framework;
using Rock.KeyValueStores;

namespace Rock.Core.IntegrationTests.KeyValueStores
{
    public abstract class DirectoryBucketTestBase : FileKeyValueStoreTestBase
    {
        protected const string BucketName = "foo";
        protected static readonly string BucketPath = Path.Combine(KeyValueStorePath, BucketName);

        [SetUp]
        public new void Setup()
        {
            Bucket = (DirectoryBucket)KeyValueStore.GetBucket(BucketName);
        }

        protected DirectoryBucket Bucket { get; private set; }
    }
}