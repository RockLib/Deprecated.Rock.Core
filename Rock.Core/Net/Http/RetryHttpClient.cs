using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Net.Http
{
    public class RetryHttpClient : HttpClient
    {
        public RetryHttpClient(int maxRetries = 3)
            : base(new RetryHandler(maxRetries), true)
        {
        }

        private class RetryHandler : DelegatingHandler
        {
            private const int MaxMaxRetries = 50;
            private readonly int _maxRetries;

            public RetryHandler(int maxRetries)
                : base(new HttpClientHandler())
            {
                if (maxRetries < 0)
                {
                    throw new ArgumentException("maxRetries must be greater than or equal to zero.", "maxRetries");
                }

                if (maxRetries > MaxMaxRetries)
                {
                    throw new ArgumentException("maxRetries must not be greater than " + MaxMaxRetries + ".");
                }

                _maxRetries = maxRetries;
            }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var i = 0;
                HttpResponseMessage response;

                do
                {
                    response = await base.SendAsync(request, cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                } while (i++ < _maxRetries);

                return response;
            }
        }
    }
}