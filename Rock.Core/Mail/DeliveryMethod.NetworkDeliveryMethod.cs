using System.Net.Mail;

namespace Rock.Mail
{
    public static partial class DeliveryMethod
    {
        private class NetworkDeliveryMethod : IDeliveryMethod
        {
            private readonly string _host;
            private readonly int _port;

            public NetworkDeliveryMethod(string host, int port)
            {
                _host = host;
                _port = port;
            }

            public void ConfigureSmtpClient(SmtpClient smtpClient)
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Host = _host;
                smtpClient.Port = _port;
            }
        }
    }
}