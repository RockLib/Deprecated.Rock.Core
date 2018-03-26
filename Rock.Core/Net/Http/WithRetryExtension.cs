//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Net.Http;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Rock.Net.Http
//{
//    public static class WithRetryExtension
//    {
//        private static readonly Func<HttpClient, HttpMessageHandler> _getHandler = GetGetHandlerFunc();

//        public static HttpClient WithRetry(this HttpClient client, int maxRetries = 3)
//        {
//            var handler = _getHandler(client);
//            var retryHandler = new RetryMessageHandler(handler, maxRetries, client.Timeout);

//            return
//                new HttpClient(retryHandler)
//                {
//                    Timeout = TimeSpan.FromMilliseconds(client.Timeout.TotalMilliseconds * (maxRetries + 1))
//                };
//        }

//        public class RetryMessageHandler : DelegatingHandler
//        {
//            private readonly int _maxRetries;
//            private readonly TimeSpan _timeout;

//            public RetryMessageHandler(HttpMessageHandler handler, int maxRetries, TimeSpan timeout)
//                : base(handler)
//            {
//                _maxRetries = maxRetries;
//                _timeout = timeout;
//            }

//            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ignored)
//            {
//                var attempts = 0;

//                bool timedOut;
//                Exception lastException;
//                HttpResponseMessage response;

//                while (true)
//                {
//                    timedOut = false;
//                    lastException = null;
//                    response = null;

//                    var stopwatch = Stopwatch.StartNew();

//                    try
//                    {
//                        var tokenSource = new CancellationTokenSource(_timeout);

//                        response = await base.SendAsync(request, tokenSource.Token).ConfigureAwait(false);

//                        if (response.IsSuccessStatusCode)
//                        {
//                            return response;
//                        }
//                    }
//                    catch (TaskCanceledException)
//                    {
//                        timedOut = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        lastException = ex;
//                    }

//                    stopwatch.Stop();

//                    if (++attempts < _maxRetries)
//                    {
//                        await Task.Delay(Convert.ToInt32(Math.Max(0, _timeout.TotalMilliseconds - stopwatch.Elapsed.TotalMilliseconds))).ConfigureAwait(false);
//                    }
//                    else
//                    {
//                        break;
//                    }
//                }

//                if (response == null)
//                {
//                    // TODO: need better exceptions here
//                    if (timedOut)
//                    {
//                        throw new TimeoutException();
//                    }

//                    throw new InvalidOperationException("Unable to make request.", lastException);
//                }

//                return response;
//            }
//        }

//        private static Func<HttpClient, HttpMessageHandler> GetGetHandlerFunc()
//        {
//            var handlerField = GetHandlerField();

//            var httpClientParameter = Expression.Parameter(typeof(HttpClient), "client");

//            var lambda =
//                Expression.Lambda<Func<HttpClient, HttpMessageHandler>>(
//                    Expression.Field(httpClientParameter, handlerField),
//                    httpClientParameter);

//            return lambda.Compile();
//        }

//        // There should be a unit test on this to ensure that we don't break things if/when Microsoft's implementation changes. (you know, since we're using reflection to obtain the handler)
//        internal static FieldInfo GetHandlerField()
//        {
//            return typeof(HttpMessageInvoker).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Single(f => f.FieldType == typeof(HttpMessageHandler));
//        }
//    }
//}