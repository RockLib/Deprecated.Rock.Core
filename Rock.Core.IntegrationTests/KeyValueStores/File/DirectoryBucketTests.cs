using System.IO;
using System.Linq;
using NUnit.Framework;
using Rock.Core.IntegrationTests.KeyValueStores;

namespace DirectoryBucketTests
{
    public class TheNameProperty : DirectoryBucketTestBase
    {
        [Test]
        public void CorrespondsToTheNameOfItsDirectory()
        {
            Assert.That(Bucket.Name, Is.EqualTo(BucketName));
        }
    }

    public class TheGetItemsMethod : DirectoryBucketTestBase
    {
        [Test]
        public void ReturnsOneBucketItemPerFile()
        {
            File.Create(Path.Combine(BucketPath, "bar")).Close();
            File.Create(Path.Combine(BucketPath, "baz")).Close();

            var items = Bucket.GetItems();

            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ReturnsBucketItemsWhoseKeysCorrespondToTheirFileName()
        {
            File.Create(Path.Combine(BucketPath, "bar")).Close();

            var item = Bucket.GetItem("bar");

            Assert.That(item.Key, Is.EqualTo("bar"));
        }

        [Test]
        public void ReturnsBucketItemsWhoseBucketNamesCorrespondToTheirBucketsNames()
        {
            File.Create(Path.Combine(BucketPath, "bar")).Close();

            var item = Bucket.GetItem("bar");

            Assert.That(item.BucketName, Is.EqualTo(BucketName));
        }
    }

    public class TheGetItemMethod : DirectoryBucketTestBase
    {
        [Test]
        public void ReturnsABucketItemWhoseKeyCorrespondsToItsFileName()
        {
            var item = Bucket.GetItem("bar");

            Assert.That(item.Key, Is.EqualTo("bar"));
        }

        [Test]
        public void ReturnsABucketItemWhoseBucketNameCorrespondsToItsBucketsName()
        {
            var item = Bucket.GetItem("bar");

            Assert.That(item.BucketName, Is.EqualTo(BucketName));
        }

        [Test]
        public void StripsInvalidCharactersFromAKey()
        {
            var item = Bucket.GetItem("<bar>");

            Assert.That(item.Key, Is.EqualTo("bar"));
        }
    }
}