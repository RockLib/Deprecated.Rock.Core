using Rock.Immutable;

namespace Rock
{
    /// <summary>
    /// Provides access to the ID of the current application.
    /// </summary>
    public static class ApplicationId
    {
        private static readonly Semimutable<string> _current = new Semimutable<string>(() => new DefaultApplicationIdProvider().GetApplicationId()); 

        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        public static string Current
        {
            get { return _current.Value; }
        }

        public static void SetCurrent(string value)
        {
            _current.Value = value;
        }

        public static void SetCurrent(IApplicationIdProvider applicationIdProvider)
        {
            _current.Value = applicationIdProvider.GetApplicationId();
        }
    }
}
