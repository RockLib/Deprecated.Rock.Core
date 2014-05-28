using System.IO;
using NUnit.Framework;
using Rock.Core.IntegrationTests.KeyValueStores;

// ReSharper disable once CheckNamespace
namespace FileBucketItemTests
{
    public class TheKeyProperty : FileBucketItemTestBase
    {
        [Test]
        public void CorrespondsToTheNameOfItsFile()
        {
            Assert.That(BucketItem.Key, Is.EqualTo(Key));
        }
    }

    public class TheBucketNameProperty : FileBucketItemTestBase
    {
        [Test]
        public void CorrespondsToTheNameOfTheDirectoryOfItsFile()
        {
            Assert.That(BucketItem.BucketName, Is.EqualTo(BucketName));
        }
    }

    public class TheTryGetMethod : FileBucketItemTestBase
    {
        [Test]
        public void ReturnsFalseIfTheItemsFileDoesNotExist()
        {
            int dummy;
            Assert.That(BucketItem.TryGet(out dummy), Is.False);
        }

        [Test]
        public void ReturnsFalseIfTheContentsOfTheItemsFileCannotBeDeserialized()
        {
            CreateBucketItemFile("not an integer");

            int dummy;
            Assert.That(BucketItem.TryGet(out dummy), Is.False);
        }

        [Test]
        public void ReturnsTrueIfTheItemsFileExistsAndCanBeDeserialized()
        {
            CreateBucketItemFile(123);

            int dummy;
            Assert.That(BucketItem.TryGet(out dummy), Is.True);
        }

        [Test]
        public void WhenReturningTrueAssignsTheDeserializedValueOfTheFileToTheOutParameter()
        {
            CreateBucketItemFile(123);

            int value;
            BucketItem.TryGet(out value);

            Assert.That(value, Is.EqualTo(123));
        }
    }

    public class ThePutMethod : FileBucketItemTestBase
    {
        [Test]
        public void CreatesTheBackingFileOfTheItemIfItDoesNotAlreadyExist()
        {
            Assert.That(File.Exists(BucketItemPath), Is.False);

            BucketItem.Put(123);

            Assert.That(File.Exists(BucketItemPath), Is.True);
        }

        [Test]
        public void WhenTheBackingFileOfTheItemDoesNotExistCorrectlyWritesTheContentsOfTheNewFile()
        {
            Assert.That(File.Exists(BucketItemPath), Is.False);

            BucketItem.Put(123);

            Assert.That(GetBucketItemFileContents<int>(), Is.EqualTo(123));
        }

        [Test]
        public void WhenTheBackingFileOfTheItemDoesExistCorrectlyOverwritesContentsOfTheExistingFile()
        {
            CreateBucketItemFile(123);

            Assert.That(File.Exists(BucketItemPath), Is.True);

            BucketItem.Put(456);

            Assert.That(GetBucketItemFileContents<int>(), Is.EqualTo(456));
        }
    }

    public class TheDeleteMethod : FileBucketItemTestBase
    {
        [Test]
        public void DeletesTheBackingFileOfTheItem()
        {
            CreateBucketItemFile(123);
            Assert.That(File.Exists(BucketItemPath), Is.True);

            BucketItem.Delete();

            Assert.That(File.Exists(BucketItemPath), Is.False);
        }

        [Test]
        public void DoesNothingIfTheItemDoesNotHaveABackingFile()
        {
            Assert.That(() => BucketItem.Delete(), Throws.Nothing);
        }
    }
}