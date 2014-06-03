using System.Net.Mail;

namespace Rock.Mail
{
    public partial class DeliveryMethod
    {
        private class DefaultDeliveryMethod : DeliveryMethod
        {
            /// <summary>
            /// The singleton instance of <see cref="DeliveryMethod.DefaultDeliveryMethod"/>.
            /// </summary>
            public static readonly DefaultDeliveryMethod Instance = new DefaultDeliveryMethod();

            private DefaultDeliveryMethod()
            {
            }

            public override void ConfigureSmtpClient(SmtpClient smtpClient)
            {
            }
        }
    }
}