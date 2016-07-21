using System.Collections.Generic;
using System.Linq;

namespace Rock.BackgroundErrorLogging
{
    /// <summary>
    /// A composite implementation of <see cref="IBackgroundErrorLogger"/>.
    /// </summary>
    public class CompositeBackgroundErrorLogger : IBackgroundErrorLogger
    {
        private readonly IBackgroundErrorLogger[] _loggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeBackgroundErrorLogger"/> class.
        /// </summary>
        /// <param name="loggers">
        /// A collection of <see cref="IBackgroundErrorLogger"/> objects whose
        /// <see cref="IBackgroundErrorLogger.Log"/> methods should be called when this instance's
        /// <see cref="Log"/> method is called.
        /// </param>
        public CompositeBackgroundErrorLogger(IEnumerable<IBackgroundErrorLogger> loggers)
            : this(loggers.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeBackgroundErrorLogger"/> class.
        /// </summary>
        /// <param name="loggers">
        /// An array of <see cref="IBackgroundErrorLogger"/> objects whose
        /// <see cref="IBackgroundErrorLogger.Log"/> methods should be called when this instance's
        /// <see cref="Log"/> method is called.
        /// </param>
        public CompositeBackgroundErrorLogger(params IBackgroundErrorLogger[] loggers)
        {
            _loggers = loggers;
        }

        /// <summary>
        /// Calls the <see cref="IBackgroundErrorLogger.Log"/> method on each of the loggers that
        /// were passed in to the constructor of this object.
        /// </summary>
        /// <param name="log">The error log.</param>
        public void Log(BackgroundErrorLog log)
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(log);
                } // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }
    }
}