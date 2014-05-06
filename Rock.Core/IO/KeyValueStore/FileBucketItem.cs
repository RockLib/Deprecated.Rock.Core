using System.IO;
using Rock.Serialization;

namespace Rock.IO
{
    public class FileBucketItem : IBucketItem
    {
        private readonly ISerializer _serializer;

        private readonly FileInfo _fileInfo;
        private readonly string _bucketName;

        public FileBucketItem(ISerializer serializer, FileInfo fileInfo)
        {
            _serializer = serializer;

            _fileInfo = fileInfo;
            _bucketName =
                _fileInfo.Directory != null
                    ? _fileInfo.Directory.Name
                    : null;
        }

        public string BucketName { get { return _bucketName; } }
        public string Key { get { return _fileInfo.Name; } }

        public bool TryGetValue<T>(out T value)
        {
            if (!_fileInfo.Exists)
            {
                value = default(T);
                return false;
            }

            using (var stream = _fileInfo.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    value = _serializer.Deserialize<T>(reader);
                    return true;
                }
            }
        }

        public void SetValue<T>(T value)
        {
            using (var stream = _fileInfo.OpenWrite())
            {
                using (var writer = new StreamWriter(stream))
                {
                    _serializer.Serialize(writer, value);
                }
            }
        }

        public void Delete()
        {
            _fileInfo.Delete();
        }
    }
}