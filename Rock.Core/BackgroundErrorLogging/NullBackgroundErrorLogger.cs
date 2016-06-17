namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="IBackgroundErrorLogger"/> that does nothing.
    /// </summary>
    public class NullBackgroundErrorLogger : IBackgroundErrorLogger
    {
        private static readonly NullBackgroundErrorLogger _instance = new NullBackgroundErrorLogger();

        private NullBackgroundErrorLogger()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="NullBackgroundErrorLogger"/> class.
        /// </summary>
        public static NullBackgroundErrorLogger Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="message">Ignored.</param>
        public void Log(BackgroundErrorLogMessage message)
        {
        }
    }
}