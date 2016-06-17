using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Rock.Immutable;

namespace Rock.BackgroundErrorLogging
{
    // ReSharper disable ExplicitCallerInfoArgument
    /// <summary>
    /// Defines methods for logging background errors.
    /// </summary>
    public static class BackgroundErrorLogger
    {
        private static readonly Semimutable<IBackgroundErrorLogger> _current = new Semimutable<IBackgroundErrorLogger>(GetDefaultLibraryLogger, true);

        /// <summary>
        /// Gets the current <see cref="IBackgroundErrorLogger"/>.
        /// </summary>
        public static IBackgroundErrorLogger Current
        {
            get { return _current.Value; }
        }

        /// <summary>
        /// Sets the current <see cref="IBackgroundErrorLogger"/>.
        /// </summary>
        /// <param name="value">An instance of <see cref="IBackgroundErrorLogger"/>.</param>
        public static void SetCurrent(IBackgroundErrorLogger value)
        {
            _current.Value = value ?? GetDefaultLibraryLogger();
        }

        internal static void UnlockCurrent()
        {
            _current.UnlockValue();
        }

        private static IBackgroundErrorLogger GetDefaultLibraryLogger()
        {
            var libraryLoggerTypeString = ConfigurationManager.AppSettings["Rock.BackgroundErrorLogging.BackgroundErrorLogger.Current"];

            IBackgroundErrorLogger backgroundErrorLogger = null;

            if (libraryLoggerTypeString != null)
            {
                var libraryLoggerType = Type.GetType(libraryLoggerTypeString);
                if (libraryLoggerType != null && typeof(IBackgroundErrorLogger).IsAssignableFrom(libraryLoggerType))
                {
                    try
                    {
                        backgroundErrorLogger = (IBackgroundErrorLogger)Activator.CreateInstance(libraryLoggerType);
                    }
                    catch
                    {
                        backgroundErrorLogger = null;
                    }
                }
            }

            return backgroundErrorLogger ?? NullBackgroundErrorLogger.Instance;
        }

        /// <summary>
        /// Logs the specified background error message.
        /// </summary>
        /// <param name="message">A message describing the background error.</param>
        /// <param name="libraryId">The id of the library that is logging the error.</param>
        /// <param name="callerMemberName">Do not provide a value for this parameter.</param>
        /// <param name="callerFilePath">Do not provide a value for this parameter.</param>
        /// <param name="callerLineNumber">Do not provide a value for this parameter.</param>
        public static void Log(
            string message,
            string libraryId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(null, message, libraryId, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs the specified background error message.
        /// </summary>
        /// <param name="exception">The background error.</param>
        /// <param name="message">An optional message.</param>
        /// <param name="libraryId">The id of the library that is logging the error.</param>
        /// <param name="callerMemberName">Do not provide a value for this parameter.</param>
        /// <param name="callerFilePath">Do not provide a value for this parameter.</param>
        /// <param name="callerLineNumber">Do not provide a value for this parameter.</param>
        public static void Log(
            Exception exception,
            string message = null,
            string libraryId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(new BackgroundErrorLogMessage(callerMemberName, callerFilePath, callerLineNumber)
            {
                Message = message,
                Exception = exception,
                LibraryName = libraryId
            });
        }

        /// <summary>
        /// Logs the specified background error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(BackgroundErrorLogMessage message)
        {
            try
            {
                Current.Log(message);
            } // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }
    }
}
// ReSharper restore ExplicitCallerInfoArgument