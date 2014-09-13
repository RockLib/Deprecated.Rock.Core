using System.Net.Http;

namespace Rock.Net.Http
{
    public interface IHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}
