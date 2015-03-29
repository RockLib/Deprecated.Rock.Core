using System;

namespace Rock
{
    /// <summary>
    /// The default implementation of <see cref="IApplicationIdProvider"/>. Attempts to get the
    /// value from an instance of <see cref="AppSettingsApplicationIdProvider"/>. If that fails,
    /// gets the value from an instance of <see cref="EntryAssemblyApplicationIdProvider"/>.
    /// </summary>
    internal class DefaultApplicationIdProvider : IApplicationIdProvider
    {
        private readonly Lazy<string> _appId = new Lazy<string>(() =>
            GetApplicationId(new AppSettingsApplicationIdProvider())
            ?? GetApplicationId(new EntryAssemblyApplicationIdProvider()));

        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        public string GetApplicationId()
        {
            return _appId.Value;
        }

        private static string GetApplicationId(IApplicationIdProvider applicationIdProvider)
        {
            try
            {
                return applicationIdProvider.GetApplicationId();
            }
            catch
            {
                return null;
            }
        }
    }
}
