using System.Net;

namespace Rock.Net
{
    public class EndpointStatus
    {
        private readonly bool _success;
        private readonly string _endpoint;
        private readonly HttpStatusCode? _statusCode;

        public EndpointStatus(bool success, string endpoint, HttpStatusCode? statusCode = null)
        {
            _success = success;
            _endpoint = endpoint;
            _statusCode = statusCode;
        }

        public bool Success
        {
            get { return _success; }
        }

        public string Endpoint
        {
            get { return _endpoint; }
        }

        public HttpStatusCode? StatusCode
        {
            get { return _statusCode; }
        }
    }
}