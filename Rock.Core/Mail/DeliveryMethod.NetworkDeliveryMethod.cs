using System.Net.Mail;

namespace Rock.Mail
{
    public partial class DeliveryMethod
    {
        private class NetworkDeliveryMethod : DeliveryMethod
        {
            private readonly string _host;
            private readonly int _port;

            public NetworkDeliveryMethod(string host, int port)
            {
                _host = host;
                _port = port;
            }

            public override void ConfigureSmtpClient(SmtpClient smtpClient)
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Host = _host;
                smtpClient.Port = _port;
            }
        }
    }
}