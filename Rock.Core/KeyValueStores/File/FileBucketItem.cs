using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using Rock.Serialization;

namespace Rock.KeyValueStores
{
    [Serializable]
    [DataContract]
    public class FileBucketItem : IBucketItem
    {
        [NonSerialized]
        private Mutex _mutex;

        [NonSerialized]
        private readonly ISerializer _serializer;

        [DataMember]
        private readonly FileInfo _fileInfo;

        [DataMember]
        private readonly string _bucketName;

        public FileBucketItem(ISerializer serializer, FileInfo fileInfo, string bucketName)
        {
            _serializer = serializer;

            _fileInfo = fileInfo;
            _bucketName = bucketName;

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
                    try
                    {
                        value = _serializer.DeserializeFromStream<T>(stream);
                        return true;
                    }
                    catch
                    {
                        value = default(T);
                        return false;
                    }
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
                    _serializer.SerializeToStream(stream, value);
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