using System.Net.Http;

namespace Rock.Net.Http
{
    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateHttpClient()
        {
            return new HttpClient();
        }
    }
}
