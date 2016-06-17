using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Rock.Configuration.Xml;
using Rock.Immutable;

namespace Rock.LibraryLogging
{
    // ReSharper disable ExplicitCallerInfoArgument
    public static class LibraryLogger
    {
        private static readonly Semimutable<ILibraryLogger> _current = new Semimutable<ILibraryLogger>(GetDefaultLibraryLogger, true);

        public static ILibraryLogger Current
        {
            get { return _current.Value; }
        }

        public static void SetCurrent(ILibraryLogger value)
        {
            _current.Value = value ?? GetDefaultLibraryLogger();
        }

        internal static void UnlockCurrent()
        {
            _current.UnlockValue();
        }

        private static ILibraryLogger GetDefaultLibraryLogger()
        {
            var configurationProxy = (LibraryLoggerConfigurationProxy)ConfigurationManager.GetSection("rock.librarylogging");

            if (configurationProxy != null)
            {
                var configuration = configurationProxy.CreateInstance();
                return configuration.LibraryLogger ?? NullLibraryLogger.Instance;
            }

            return NullLibraryLogger.Instance;
        }

        public static void Log(
            string message,
            string libraryId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(null, message, libraryId, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Log(
            Exception exception,
            string message = null,
            string libraryId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(new LibraryLogMessage(callerMemberName, callerFilePath, callerLineNumber)
            {
                Message = message,
                Exception = exception,
                LibraryName = libraryId
            });
        }

        public static void Log(LibraryLogMessage message)
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