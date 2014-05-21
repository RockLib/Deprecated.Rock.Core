using System.IO;
using System.Linq;
using NUnit.Framework;
using Rock.Core.IntegrationTests.KeyValueStores;

namespace FileKeyValueStoreTests
{
    public class AnInstanceOfFileKeyValueStore : FileKeyValueStoreTestBase
    {
        [Test]
        public void WhenCreatedCreatesItsDirectoryIfItDoesNotExist()
        {
            Assert.That(Directory.Exists(KeyValueStorePath), Is.False);

            CreateKeyValueStore();

            Assert.That(Directory.Exists(KeyValueStorePath), Is.True);
        }

        private void CreateKeyValueStore()
        {
            KeyValueStore.GetBuckets();
        }
    }

    public class TheGetBucketsMethod : FileKeyValueStoreTestBase
    {
        [Test]
        public void ReturnsOneBucketPerDirectory()
        {
            Directory.CreateDirectory(Path.Combine(KeyValueStorePath, "foo"));
            Directory.CreateDirectory(Path.Combine(KeyValueStorePath, "bar"));

            var buckets = KeyValueStore.GetBuckets();

            Assert.That(buckets.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ReturnsBucketsWhoseNamesCorrespondToTheirDirectory()
        {
            Directory.CreateDirectory(Path.Combine(KeyValueStorePath, "foo"));

            var buckets = KeyValueStore.GetBuckets();

            var bucket = buckets.First();
            Assert.That(bucket.Name, Is.EqualTo("foo"));
        }
    }

    public class TheGetBucketMethod : FileKeyValueStoreTestBase
    {
        [Test]
        public void CreatesTheBucketsDirectoryIfItDoesNotExist()
        {
            var bucketDirectoryPath = Path.Combine(KeyValueStorePath, "foo");

            Assert.That(Directory.Exists(bucketDirectoryPath), Is.False);

            KeyValueStore.GetBucket("foo");

            Assert.That(Directory.Exists(bucketDirectoryPath), Is.True);
        }

        [Test]
        public void ReturnsABucketWhoseNameCorrespondsToItsDirectory()
        {
            var bucket = KeyValueStore.GetBucket("foo");

            Assert.That(bucket.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void StripsInvalidCharactersFromABucketName()
        {
            var bucket = KeyValueStore.GetBucket("<foo>");

            Assert.That(bucket.Name, Is.EqualTo("foo"));
        }
    }
}