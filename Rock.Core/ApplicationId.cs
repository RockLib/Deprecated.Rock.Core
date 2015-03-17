using Rock.Defaults.Implementation;
using System;
using System.Configuration;

namespace Rock
{
    public static class ApplicationId
    {
        public static string Current
        {
            get { return Default.ApplicationInfo.ApplicationId; }
        }
    }
}
