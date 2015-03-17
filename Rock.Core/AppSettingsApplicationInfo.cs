using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock
{
    public class AppSettingsApplicationInfo : IApplicationInfo
    {
        private const string DefaultApplicationIdKey = "Rock.ApplicationId.Current";

        private readonly string _applicationIdKey;

        public AppSettingsApplicationInfo()
            : this(DefaultApplicationIdKey)
        {
        }

        public AppSettingsApplicationInfo(string applicationIdKey)
        {
            _applicationIdKey = applicationIdKey;
        }

        public string ApplicationId
        {
            get { return ConfigurationManager.AppSettings[_applicationIdKey]; }
        }
    }
}
