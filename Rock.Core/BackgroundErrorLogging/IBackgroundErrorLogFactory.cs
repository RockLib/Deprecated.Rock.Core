namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// Defines an interface for creating instances of <see cref="BackgroundErrorLog"/> objects.
    /// </summary>
    public interface IBackgroundErrorLogFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="BackgroundErrorLog"/>.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to a background error logger method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller to a background error logger method.</param>
        /// <param name="callerLineNumber">The line number in the source file at which a background error logger method is called.</param>
        /// <returns></returns>
        BackgroundErrorLog Create(string callerMemberName, string callerFilePath, int callerLineNumber);
    }
}