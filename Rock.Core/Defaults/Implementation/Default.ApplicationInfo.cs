using System;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IApplicationInfo> _applicationInfo = new DefaultHelper<IApplicationInfo>(() => new EntryAssemblyApplicationInfo());

        public static IApplicationInfo ApplicationInfo
        {
            get { return _applicationInfo.Current; }
        }

        public static IApplicationInfo DefaultApplicationInfo
        {
            get { return _applicationInfo.DefaultInstance; }
        }

        public static void SetApplicationInfo(Func<IApplicationInfo> getApplicationInfoInstance)
        {
            _applicationInfo.SetCurrent(getApplicationInfoInstance);
        }

        public static void RestoreDefaultApplicationInfo()
        {
            _applicationInfo.RestoreDefault();
        }
    }
}