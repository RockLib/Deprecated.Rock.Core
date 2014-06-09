using System;
using System.Threading.Tasks;

namespace Rock
{
    public interface IExceptionHandler
    {
        Task HandleException(Exception ex);
    }
}