using System;

namespace Rock.DependencyInjection
{
    public class ResolveException : Exception
    {
        public ResolveException()
        {
        }

        public ResolveException(string message)
            : base(message)
        {
        }

        public ResolveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}