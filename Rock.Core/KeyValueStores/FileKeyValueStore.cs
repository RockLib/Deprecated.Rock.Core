using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rock.Serialization;

namespace Rock.KeyValueStores
{
    public class FileKeyValueStore : IKeyValueStore
    {
        private static readonly Regex _invalidPathChars =
            new Regex(
                string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidPathChars()))),
                RegexOptions.Compiled);

        private readonly ISerializer _serializer;
        private readonly DirectoryInfo _directoryInfo;

        public FileKeyValueStore(ISerializer serializer, DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            _serializer = serializer;
            _directoryInfo = directoryInfo;
        }

        public IEnumerable<IBucket> GetBuckets()
        {
            return
                _directoryInfo.GetDirectories()
                    .Select(subDirectoryInfo => new DirectoryBucket(_serializer, subDirectoryInfo));
        }

        public IBucket GetBucket(string bucketName)
        {
            var subDirectoryInfo = new DirectoryInfo(Path.Combine(_directoryInfo.FullName, GetDirectoryName(bucketName)));

            if (!subDirectoryInfo.Exists)
            {
                subDirectoryInfo.Create();
            }

            return new DirectoryBucket(_serializer, subDirectoryInfo);
        }

        private static string GetDirectoryName(string bucketName)
        {
            return _invalidPathChars.Replace(bucketName, "");
        }

        public override int GetHashCode()
        {
            return (GetType().FullName.GetHashCode() * 397) ^ _directoryInfo.FullName.GetHashCode().GetHashCode();
        }
    }
}