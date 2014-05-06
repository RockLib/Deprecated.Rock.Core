using System;
using System.IO;
using Rock.IO;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IKeyValueStorage> _defaultTempStorage = new Default<IKeyValueStorage>(() =>
        {
            var tempDirectory = Environment.GetEnvironmentVariable("Temp");
            var tempStorageDirectoryInfo = new DirectoryInfo(Path.Combine(tempDirectory, "Rock", _defaultApplicationInfo.DefaultInstance.ApplicationId));
            return new FileKeyValueStorage(_defaultJsonSerializer.DefaultInstance, tempStorageDirectoryInfo);
        });

        public static IKeyValueStorage TempStorage
        {
            get { return _defaultTempStorage.Current; }
        }

        public static void SetTempStorage(Func<IKeyValueStorage> getTempStorageInstance)
        {
            _defaultTempStorage.SetCurrent(getTempStorageInstance);
        }
    }
}