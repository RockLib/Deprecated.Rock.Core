namespace Rock.Logging.Library
{
    public class NullLibraryLogger : ILibraryLogger
    {
        public static readonly NullLibraryLogger Instance = new NullLibraryLogger();

        private NullLibraryLogger()
        {
        }

        public void Log(ILibraryLogMessage message)
        {
        }

        public void Debug(ILibraryLogMessage message)
        {
        }

        public void Dispose()
        {
        }
    }
}