using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Rock.Immutable;

namespace Rock.Logging.Library
{
    public static class LibraryLogger
    {
        private static readonly Semimutable<ILibraryLogger> _current = new Semimutable<ILibraryLogger>(() => NullLibraryLogger.Instance);
        private static readonly Semimutable<bool> _isDebugEnabled = new Semimutable<bool>(GetDefaultIsDebugEnabled);

        public static ILibraryLogger Current
        {
            get { return _current.Value; }
        }

        public static void SetCurrent(ILibraryLogger value)
        {
            _current.Value = value ?? NullLibraryLogger.Instance;
        }

        public static bool IsEnabled
        {
            get { return Current != NullLibraryLogger.Instance; }
        }

        public static bool IsDebugEnabled
        {
            get { return _isDebugEnabled.Value; }
            set { _isDebugEnabled.Value = value; }
        }

        private static bool GetDefaultIsDebugEnabled()
        {
            var isDebugEnabledString = ConfigurationManager.AppSettings["Rock.Logging.Library.LibraryLogger.IsDebugEnabled"];

            if (!string.IsNullOrEmpty(isDebugEnabledString))
            {
                bool isDebugEnabled;
                return bool.TryParse(isDebugEnabledString, out isDebugEnabled) && isDebugEnabled;
            }

            return false;
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
                LibraryId = libraryId
            });
        }

        public static void Log(ILibraryLogMessage message)
        {
            try
            {
                Current.Log(message);
            }
            catch
            {
            }
        }

        public static void Debug(
            string message,
            string libraryId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Debug(null, message, libraryId, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Debug(
            Exception exception,
            string message = null,
            string libraryId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Debug(new LibraryLogMessage(callerMemberName, callerFilePath, callerLineNumber)
            {
                Message = message,
                Exception = exception,
                LibraryId = libraryId
            });
        }

        public static void Debug(ILibraryLogMessage message)
        {
            try
            {
                Current.Debug(message);
            }
            catch
            {
            }
        }
    }
}