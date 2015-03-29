using System.Configuration;

namespace Rock
{
    /// <summary>
    /// An implementation of <see cref="IApplicationIdProvider"/> that uses a config
    /// file's appSettings section.
    /// </summary>
    public class AppSettingsApplicationIdProvider : IApplicationIdProvider
    {
        private const string DefaultApplicationIdKey = "Rock.ApplicationId.Current";

        private readonly string _applicationIdKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsApplicationIdProvider"/> class.
        /// The key of the application ID setting will be "Rock.ApplicationId.Current".
        /// </summary>
        public AppSettingsApplicationIdProvider()
            : this(DefaultApplicationIdKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsApplicationIdProvider"/> class.
        /// </summary>
        /// <param name="applicationIdKey">The key of the application ID setting.</param>
        public AppSettingsApplicationIdProvider(string applicationIdKey)
        {
            _applicationIdKey = applicationIdKey;
        }

        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        public string GetApplicationId()
        {
            return ConfigurationManager.AppSettings[_applicationIdKey];
        }
    }
}
