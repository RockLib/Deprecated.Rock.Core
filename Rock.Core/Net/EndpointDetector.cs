using System;
using System.Net;
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
            private readonly int _timeoutMilliseconds;

            public ManualHttpWebRequest(string endpoint, int timeoutSeconds)
            {
                _endpoint = endpoint;

                var url = new Uri(endpoint);
                _server = url.Host;
                _port = url.Port;
                _path = url.AbsolutePath;

                _timeoutMilliseconds = timeoutSeconds * 1000;
            }

            public async Task<EndpointStatus> GetStatus()
            {
                try
                {
                    return await GetStatusImpl();
                }
                catch
                {
                    return new EndpointStatus(false, _endpoint);
                }
            }

            private async Task<EndpointStatus> GetStatusImpl()
            {
                var request = string.Format("GET {2} HTTP/1.1{0}Host: {1}{0}Connection: Close{0}{0}", "\r\n", _server, _path);

                var bytesSent = Encoding.ASCII.GetBytes(request);
                var bytesReceived = new Byte[256];

                var socket = await ConnectSocket();

                if (socket == null)
                {
                    return new EndpointStatus(false, _endpoint);
                }

                socket.SendTimeout = _timeoutMilliseconds;
                socket.ReceiveTimeout = _timeoutMilliseconds;

                await Task.Factory.FromAsync(
                    (callback, state) => socket.BeginSend(bytesSent, 0, bytesSent.Length, SocketFlags.None, callback, state),
                    result => socket.EndSend(result),
                    null);

                int bytes;
                var rawResponse = new StringBuilder();

                EndpointStatus endpointStatus = null;

                do
                {
                    bytes =
                        await Task.Factory.FromAsync(
                            (callback, o) =>
                            socket.BeginReceive(bytesReceived, 0, bytesReceived.Length, SocketFlags.None, callback, o),
                            result => socket.EndReceive(result),
                            null);

                    rawResponse.Append(Encoding.ASCII.GetString(bytesReceived, 0, bytes));

                    var statusLineMatch = _statusLineRegex.Match(rawResponse.ToString());
                    if (statusLineMatch.Success)
                    {
                        // As soon as we have the status line, we're done.
                        var statusCode = (HttpStatusCode)int.Parse(statusLineMatch.Groups[_statusCode].Value);
                        endpointStatus = new EndpointStatus(true, _endpoint, statusCode);
                        break;
                    }
                } while (bytes > 0);

                await Task.Factory.FromAsync(
                    (callback, o) => socket.BeginDisconnect(false, callback, o),
                    socket.EndDisconnect,
                    null).ContinueWith(task => socket.Dispose());

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