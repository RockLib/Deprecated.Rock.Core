using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rock.Net
{
    public class EndpointDetector : IEndpointDetector
    {
        public Task<EndpointStatus> GetEndpointStatus(int timeoutSeconds, string endpoint)
        {
            var request = new ManualHttpWebRequest(endpoint, timeoutSeconds);
            return request.GetStatus();
        }

        private class ManualHttpWebRequest
        {
            private const string _statusCode = "status_code";
            private const string _statusLinePattern = @"^HTTP/\d+\.\d+ (?<" + _statusCode + @">\d\d\d) [^\r\n]*\r\n";
            private static readonly Regex _statusLineRegex = new Regex(_statusLinePattern, RegexOptions.Compiled);

            private readonly string _endpoint;

            private readonly string _server;
            private readonly int _port;
            private readonly string _path;
            private readonly bool _isHttps;
            private readonly int _timeoutMilliseconds;

            public ManualHttpWebRequest(string endpoint, int timeoutSeconds)
            {
                _endpoint = endpoint;

                var url = new Uri(endpoint);
                _server = url.Host;
                _port = url.Port;
                _path = url.AbsolutePath;
                _isHttps = url.Scheme.ToLower() == "https";

                _timeoutMilliseconds = timeoutSeconds * 1000;
            }

            public async Task<EndpointStatus> GetStatus()
            {
                try
                {
                    var socket = await ConnectSocket();

                    if (socket == null)
                    {
                        return new EndpointStatus(false, _endpoint);
                    }

                    socket.SendTimeout = _timeoutMilliseconds;
                    socket.ReceiveTimeout = _timeoutMilliseconds;

                    EndpointStatus endpointStatus;

                    using (var networkStream = new NetworkStream(socket, true))
                    {
                        Stream stream = networkStream;

                        if (_isHttps)
                        {
                            var sslStream = new SslStream(networkStream);
                            await sslStream.AuthenticateAsClientAsync(_server);
                            stream = sslStream;
                        }

                        endpointStatus = await GetStatus(stream);
                    }

                    return endpointStatus;
                }
                catch
                {
                    return new EndpointStatus(false, _endpoint);
                }
            }

            private async Task<EndpointStatus> GetStatus(Stream stream)
            {
                var request = string.Format("GET {2} HTTP/1.1{0}Host: {1}{0}Connection: Close{0}{0}", "\r\n", _server, _path);

                var sendBuffer = Encoding.ASCII.GetBytes(request);
                var receiveBuffer = new Byte[256];

                await stream.WriteAsync(sendBuffer, 0, sendBuffer.Length);

                int receiveBufferLength;
                var rawResponse = new StringBuilder();

                EndpointStatus endpointStatus = null;

                do
                {
                    receiveBufferLength = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);

                    rawResponse.Append(Encoding.ASCII.GetString(receiveBuffer, 0, receiveBufferLength));

                    var statusLineMatch = _statusLineRegex.Match(rawResponse.ToString());
                    if (statusLineMatch.Success)
                    {
                        // As soon as we have the status line, we're done.
                        var statusCode = (HttpStatusCode)int.Parse(statusLineMatch.Groups[_statusCode].Value);
                        endpointStatus = new EndpointStatus(true, _endpoint, statusCode);
                        break;
                    }
                } while (receiveBufferLength > 0);

                return endpointStatus ?? new EndpointStatus(false, _endpoint);
            }

            private async Task<Socket> ConnectSocket()
            {
                var hostEntry = Dns.GetHostEntry(_server);

                foreach (var address in hostEntry.AddressList)
                {
                    var ipe = new IPEndPoint(address, _port);
                    var socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    await Task.Factory.FromAsync((callback, state) => socket.BeginConnect(ipe, callback, state), socket.EndConnect, null);

                    if (socket.Connected)
                    {
                        return socket;
                    }
                }

                return null;
            }
        }
    }
}