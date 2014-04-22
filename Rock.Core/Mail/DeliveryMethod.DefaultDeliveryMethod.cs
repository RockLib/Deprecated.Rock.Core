using System.Net.Mail;

namespace Rock.Mail
{
    public static partial class DeliveryMethod
    {
        private class DefaultDeliveryMethod : IDeliveryMethod
        {
            public static readonly DefaultDeliveryMethod Instance = new DefaultDeliveryMethod();

            private DefaultDeliveryMethod()
            {
            }

            public void ConfigureSmtpClient(SmtpClient smtpClient)
            {
            }
        }
    }
}