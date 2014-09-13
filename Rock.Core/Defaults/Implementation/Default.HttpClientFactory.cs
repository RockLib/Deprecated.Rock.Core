using System;
using Rock.Net.Http;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IHttpClientFactory> _httpClientFactory = new DefaultHelper<IHttpClientFactory>(() => new DefaultHttpClientFactory());

        public static IHttpClientFactory HttpClientFactory
        {
            get { return _httpClientFactory.Current; }
        }

        public static IHttpClientFactory DefaultHttpClientFactory
        {
            get { return _httpClientFactory.DefaultInstance; }
        }

        public static void SetHttpClientFactory(Func<IHttpClientFactory> getHttpClientFactoryInstance)
        {
            _httpClientFactory.SetCurrent(getHttpClientFactoryInstance);
        }

        public static void RestoreDefaultHttpClientFactory()
        {
            _httpClientFactory.RestoreDefault();
        }
    }
}