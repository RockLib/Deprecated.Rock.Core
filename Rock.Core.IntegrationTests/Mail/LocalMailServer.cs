using System;
using System.Threading;
using System.Threading.Tasks;
using netDumbster.smtp;

namespace Rock.Core.IntegrationTests.Mail
{
    public class LocalMailServer : IDisposable
    {
        private readonly TaskCompletionSource<string> _completion;
        private readonly CancellationTokenSource _timeoutCancelationToken;

        private readonly SimpleSmtpServer _server;

        private LocalMailServer(int port)
        {
            _completion = new TaskCompletionSource<string>();
            _timeoutCancelationToken = new CancellationTokenSource();

            _server = SimpleSmtpServer.Start(port);
            _server.MessageReceived += OnMessageReceived;
        }

        public static LocalMailServer StartNew(int? port = null)
        {
            return new LocalMailServer(port ?? SimpleSmtpServer.GetRandomUnusedPort());
        }

        public int Port
        {
            get { return _server.Port; }
        }

        public Task<string> GetMailData(int timeoutMilliseconds = 30000)
        {
            StartTimeoutTimer(timeoutMilliseconds);

            return _completion.Task;
        }

        private void StartTimeoutTimer(int timeoutMilliseconds)
        {
            if (timeoutMilliseconds > 0)
            {
                Task.Delay(timeoutMilliseconds, _timeoutCancelationToken.Token)
                    .ContinueWith(
                        t =>
                        {
                            if (!t.IsCanceled)
                            {
                                _completion.TrySetCanceled();
                            }
                        });
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedArgs args)
        {
            _timeoutCancelationToken.Cancel();
            _completion.TrySetResult(args.Message.Data);
        }

        public void Dispose()
        {
            _server.Stop();
        }
    }
}
