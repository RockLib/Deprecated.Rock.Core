namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// Defines an interface for logging background errors.
    /// </summary>
    public interface IBackgroundErrorLogger
    {
        /// <summary>
        /// Logs the specified error.
        /// </summary>
        /// <param name="log">The error log.</param>
        void Log(BackgroundErrorLog log);
    }
}