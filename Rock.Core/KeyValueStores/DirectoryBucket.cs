using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rock.Serialization;

namespace Rock.KeyValueStores
{
    public class DirectoryBucket : IBucket
    {
        private static readonly Regex _invalidFileNameChars =
            new Regex(
                string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))),
                RegexOptions.Compiled);

        private readonly ISerializer _serializer;

        private readonly DirectoryInfo _directoryInfo;

        public DirectoryBucket(ISerializer serializer, DirectoryInfo directoryInfo)
        {
            _serializer = serializer;
            _directoryInfo = directoryInfo;
        }

        public string Name { get { return _directoryInfo.Name; } }

        public IEnumerable<IBucketItem> GetItems()
        {
            return
                _directoryInfo.GetFiles()
                    .Select(fileInfo => new FileBucketItem(_serializer, fileInfo));
        }

        public IBucketItem GetItem(string key)
        {
            var fileInfo = new FileInfo(Path.Combine(_directoryInfo.FullName, GetFileName(key)));
            return new FileBucketItem(_serializer, fileInfo);
        }

        private static string GetFileName(string key)
        {
            return _invalidFileNameChars.Replace(key, "");
        }
    }
}