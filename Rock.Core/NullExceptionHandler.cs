using System;
using System.Threading.Tasks;

namespace Rock
{
    public sealed class NullExceptionHandler : IExceptionHandler
    {
        private static readonly NullExceptionHandler _instance = new NullExceptionHandler();

        private readonly Task _completedTask = Task.FromResult(0);

        private NullExceptionHandler()
        {
        }

        public static NullExceptionHandler Instance
        {
            get { return _instance; }
        }

        public Task HandleException(Exception ex)
        {
            return _completedTask;
        }
    }
}