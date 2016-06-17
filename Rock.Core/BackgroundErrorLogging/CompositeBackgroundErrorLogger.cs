using System.Collections.Generic;
using System.Linq;

namespace Rock.BackgroundErrorLogging
{
    public class CompositeBackgroundErrorLogger : IBackgroundErrorLogger
    {
        private readonly IBackgroundErrorLogger[] _loggers;

        public CompositeBackgroundErrorLogger(IEnumerable<IBackgroundErrorLogger> loggers)
            : this(loggers.ToArray())
        {
        }

        public CompositeBackgroundErrorLogger(params IBackgroundErrorLogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Log(BackgroundErrorLogMessage message)
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