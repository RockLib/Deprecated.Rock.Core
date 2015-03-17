using Rock.Defaults.Implementation;
using System;
using System.Configuration;

namespace Rock
{
    /// <summary>
    /// Provides access to the ID of the current application.
    /// </summary>
    public static class ApplicationId
    {
        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        /// <remarks>
        /// This property returns the value of <see cref="Default.ApplicationInfo.ApplicationId"/>.
        /// </remarks>
        public static string Current
        {
            get { return Default.ApplicationInfo.ApplicationId; }
        }
    }
}
