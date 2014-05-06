using System;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IApplicationInfo> _defaultApplicationInfo = new Default<IApplicationInfo>(() => new EntryAssemblyApplicationInfo());

        public static IApplicationInfo ApplicationInfo
        {
            get { return _defaultApplicationInfo.Current; }
        }

        public static void SetApplicationInfo(Func<IApplicationInfo> getApplicationInfoInstance)
        {
            _defaultApplicationInfo.SetCurrent(getApplicationInfoInstance);
        }
    }
}