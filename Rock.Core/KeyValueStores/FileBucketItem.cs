using System;
using System.IO;
using System.Threading;
using Rock.Serialization;

namespace Rock.KeyValueStores
{
    [Serializable]
    public class FileBucketItem : IBucketItem
    {
        [NonSerialized]
        private Mutex _mutex;

        [NonSerialized]
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

            _mutex = CreateMutex();
        }

        public string BucketName { get { return _bucketName; } }
        public string Key { get { return _fileInfo.Name; } }

        public bool TryGet<T>(out T value)
        {
            if (!_fileInfo.Exists)
            {
                value = default(T);
                return false;
            }

            var mutex = GetMutex();

            try
            {
                mutex.WaitOne();

                using (var stream = _fileInfo.OpenRead())
                {
                    value = _serializer.Deserialize<T>(stream);
                    return true;
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void Put<T>(T value)
        {
            var mutex = GetMutex();

            try
            {
                mutex.WaitOne();

                using (var stream = _fileInfo.OpenWrite())
                {
                    _serializer.Serialize(stream, value);
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void Delete()
        {
            var mutex = GetMutex();

            try
            {
                mutex.WaitOne();

                _fileInfo.Delete();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private Mutex GetMutex()
        {
            return _mutex ?? (_mutex = CreateMutex());
        }

        private Mutex CreateMutex()
        {
            return new Mutex(false, _fileInfo.FullName.Replace(Path.DirectorySeparatorChar, '_'));
        }
    }
}