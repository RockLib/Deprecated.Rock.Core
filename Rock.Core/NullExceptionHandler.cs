using System;

namespace Rock
{
    public sealed class NullExceptionHandler : IExceptionHandler
    {
        private static readonly NullExceptionHandler _instance = new NullExceptionHandler();

        private NullExceptionHandler()
        {
        }

        public static NullExceptionHandler Instance
        {
            get { return _instance; }
        }

        public void HandleException(Exception ex)
        {
        }
    }
}