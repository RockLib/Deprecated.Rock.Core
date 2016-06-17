using System.Collections.Generic;
using System.Linq;

namespace Rock.LibraryLogging
{
    public class CompositeLibraryLogger : ILibraryLogger
    {
        private readonly ILibraryLogger[] _loggers;

        public CompositeLibraryLogger(IEnumerable<ILibraryLogger> loggers)
            : this(loggers.ToArray())
        {
        }

        public CompositeLibraryLogger(params ILibraryLogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Log(LibraryLogMessage message)
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(message);
                } // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }
    }
}