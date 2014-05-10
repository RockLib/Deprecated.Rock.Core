using System;
using System.IO;
using Rock.KeyValueStores;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IKeyValueStore> _tempStorage = new DefaultHelper<IKeyValueStore>(() =>
        {
            var tempDirectory = Environment.GetEnvironmentVariable("Temp");
            var tempStorageDirectoryInfo = new DirectoryInfo(Path.Combine(tempDirectory, "Rock", _applicationInfo.DefaultInstance.ApplicationId));
            return new FileKeyValueStore(_jsonSerializer.DefaultInstance, tempStorageDirectoryInfo);
        });

        public static IKeyValueStore DefaultTempStorage
        {
            get { return _tempStorage.DefaultInstance; }
        }

        public static IKeyValueStore TempStorage
        {
            get { return _tempStorage.Current; }
        }

        public static void SetTempStorage(Func<IKeyValueStore> getTempStorageInstance)
        {
            _tempStorage.SetCurrent(getTempStorageInstance);
        }
    }
}