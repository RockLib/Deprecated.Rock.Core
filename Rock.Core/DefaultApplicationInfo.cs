using System;

namespace Rock
{
    /// <summary>
    /// The default implementation of <see cref="IApplicationInfo"/>. Attempts to get the
    /// value from an instance of <see cref="AppSettingsApplicationInfo"/>. If that fails,
    /// gets the value from an instance of <see cref="EntryAssemblyApplicationInfo"/>.
    /// </summary>
    internal class DefaultApplicationInfo : IApplicationInfo
    {
        private readonly Lazy<string> _appId = new Lazy<string>(() =>
            GetApplicationId(new AppSettingsApplicationInfo())
            ?? GetApplicationId(new EntryAssemblyApplicationInfo()));

        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        public string ApplicationId
        {
            get { return _appId.Value; }
        }

        private static string GetApplicationId(IApplicationInfo applicationInfo)
        {
            try
            {
                return applicationInfo.ApplicationId;
            }
            catch
            {
                return null;
            }
        }
    }
}
