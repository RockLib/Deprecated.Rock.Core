using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock
{
    internal class DefaultApplicationInfo : IApplicationInfo
    {
        private readonly Lazy<string> _appId = new Lazy<string>(() =>
            GetApplicationId(new AppSettingsApplicationInfo())
            ?? GetApplicationId(new EntryAssemblyApplicationInfo()));

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
