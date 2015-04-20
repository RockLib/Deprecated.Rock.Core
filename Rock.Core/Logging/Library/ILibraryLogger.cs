namespace Rock.Logging.Library
{
    public interface ILibraryLogger
    {
        void Log(ILibraryLogMessage message);
        void Debug(ILibraryLogMessage message); 
    }
}