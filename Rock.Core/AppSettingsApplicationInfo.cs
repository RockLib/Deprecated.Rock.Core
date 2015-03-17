using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock
{
    /// <summary>
    /// An implementation of <see cref="IApplicationInfo"/> that uses a config
    /// file's appSettings section.
    /// </summary>
    public class AppSettingsApplicationInfo : IApplicationInfo
    {
        private const string DefaultApplicationIdKey = "Rock.ApplicationId.Current";

        private readonly string _applicationIdKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsApplicationInfo"/> class.
        /// </summary>
        public AppSettingsApplicationInfo()
            : this(DefaultApplicationIdKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsApplicationInfo"/> class.
        /// </summary>
        /// <param name="applicationIdKey">The key of the application ID setting.</param>
        public AppSettingsApplicationInfo(string applicationIdKey)
        {
            _applicationIdKey = applicationIdKey;
        }

        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        public string ApplicationId
        {
            get { return ConfigurationManager.AppSettings[_applicationIdKey]; }
        }
    }
}
