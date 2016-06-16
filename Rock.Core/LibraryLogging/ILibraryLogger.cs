namespace Rock.LibraryLogging
{
    /// <summary>
    /// Defines various logging methods for libraries to use.
    /// </summary>
    public interface ILibraryLogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Log(LibraryLogMessage message);

        /// <summary>
        /// Logs the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(LibraryLogMessage message); 
    }
}