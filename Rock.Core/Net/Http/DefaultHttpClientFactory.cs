using Rock.Immutable;

namespace Rock.Net.Http
{
    public static class DefaultHttpClientFactory
    {
        private static readonly Semimutable<IHttpClientFactory> _httpClientFactory = new Semimutable<IHttpClientFactory>(GetDefault);

        public static IHttpClientFactory Current
        {
            get { return _httpClientFactory.Value; }
        }

        public static void SetCurrent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory.Value = httpClientFactory;
        }

        private static IHttpClientFactory GetDefault()
        {
            return new HttpClientFactory();
        }
    }
}
