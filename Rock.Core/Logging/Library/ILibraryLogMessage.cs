using System;

namespace Rock.Logging.Library
{
    /// <summary>
    /// Defines the various properties of a log message.
    /// </summary>
    public interface ILibraryLogMessage
    {
        /// <summary>
        /// Gets the name of the library where the log message is coming from.
        /// </summary>
        string LibraryName { get; }

        /// <summary>
        /// Gets the message of the log message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the time that the log message was created.
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// Gets the exception associated with the log message.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Gets the method or property name where the log message originated.
        /// </summary>
        string CallerMemberName { get; }

        /// <summary>
        /// Gets the full path of the source file where the log message originated.
        /// </summary>
        string CallerFilePath { get; }

        /// <summary>
        /// Gets the line number in the source file where the log message originated.
        /// </summary>
        int CallerLineNumber { get; }
    }
}