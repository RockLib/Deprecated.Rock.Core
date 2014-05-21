using System.IO;
using NUnit.Framework;
using Rock.KeyValueStores;

namespace Rock.Core.IntegrationTests.KeyValueStores
{
    public class FileBucketItemTestBase : DirectoryBucketTestBase
    {
        protected const string Key = "bar";
        protected static readonly string BucketItemPath = Path.Combine(BucketPath, Key);

        [SetUp]
        public new void Setup()
        {
            BucketItem = (FileBucketItem)Bucket.GetItem(Key);
        }

        protected FileBucketItem BucketItem { get; private set; }

        protected void CreateBucketItemFile<T>(T value)
        {
            using (var stream = File.Create(BucketItemPath))
            {
                Serializer.Serialize(stream, value, typeof(T));
            }
        }

        protected T GetBucketItemFileContents<T>()
        {
            using (var stream = File.OpenRead(BucketItemPath))
            {
                return (T)Serializer.Deserialize(stream, typeof(T));
            }
        }
    }
}