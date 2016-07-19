using System;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// An implementation of <see cref="IBackgroundErrorLogFactory"/> whose <see cref="Create"/> method
    /// is defined by the function passed in to its constructor.
    /// </summary>
    public class BackgroundErrorLogFactory : IBackgroundErrorLogFactory
    {
        private readonly Func<string, string, int, BackgroundErrorLog> _create;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundErrorLogFactory"/> class.
        /// </summary>
        /// <param name="createBackgroundErrorLogFunc">A function that is called from the <see cref="Create"/> method.</param>
        public BackgroundErrorLogFactory(Func<string, string, int, BackgroundErrorLog> createBackgroundErrorLogFunc)
        {
            _create = createBackgroundErrorLogFunc;
        }

        /// <summary>
        /// Creates an instance of <see cref="BackgroundErrorLog"/>.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to a background error logger method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller to a background error logger method.</param>
        /// <param name="callerLineNumber">The line number in the source file at which a background error logger method is called.</param>
        /// <returns></returns>
        public BackgroundErrorLog Create(string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            return _create(callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}