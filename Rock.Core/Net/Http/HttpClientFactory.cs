using System.Net.Http;

namespace Rock.Net.Http
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateHttpClient()
        {
            return new HttpClient();
        }
    }
}
