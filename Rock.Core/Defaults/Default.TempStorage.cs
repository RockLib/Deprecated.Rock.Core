using System;
using System.IO;
using Rock.IO;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IKeyValueStore> _defaultTempStorage = new Default<IKeyValueStore>(() =>
        {
            var tempDirectory = Environment.GetEnvironmentVariable("Temp");
            var tempStorageDirectoryInfo = new DirectoryInfo(Path.Combine(tempDirectory, "Rock", _defaultApplicationInfo.DefaultInstance.ApplicationId));
            return new FileKeyValueStore(_defaultJsonSerializer.DefaultInstance, tempStorageDirectoryInfo);
        });

        public static IKeyValueStore TempStorage
        {
            get { return _defaultTempStorage.Current; }
        }

        public static void SetTempStorage(Func<IKeyValueStore> getTempStorageInstance)
        {
            _defaultTempStorage.SetCurrent(getTempStorageInstance);
        }
    }
}