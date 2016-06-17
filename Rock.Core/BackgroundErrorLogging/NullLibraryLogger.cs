namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="ILibraryLogger"/> that does nothing.
    /// </summary>
    public class NullLibraryLogger : ILibraryLogger
    {
        private static readonly NullLibraryLogger _instance = new NullLibraryLogger();

        private NullLibraryLogger()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="NullLibraryLogger"/> class.
        /// </summary>
        public static NullLibraryLogger Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="message">Ignored.</param>
        public void Log(LibraryLogMessage message)
        {
        }
    }
}