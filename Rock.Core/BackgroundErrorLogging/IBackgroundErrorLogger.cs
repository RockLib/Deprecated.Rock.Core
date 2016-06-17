namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// Defines an interface for logging background errors.
    /// </summary>
    public interface IBackgroundErrorLogger
    {
        /// <summary>
        /// Logs the specified background error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Log(BackgroundErrorLogMessage message);
    }
}