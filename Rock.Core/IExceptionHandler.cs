using System;

namespace Rock
{
    public interface IExceptionHandler
    {
        void HandleException(Exception ex);
    }
}