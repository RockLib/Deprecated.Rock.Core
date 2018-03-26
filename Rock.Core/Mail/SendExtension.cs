using System.Net.Mail;
using System.Threading.Tasks;

namespace Rock.Mail
{
    public static class SendExtension
    {
        /// <summary>
        /// Asynchronously sends the specified mail message using the specified
        /// delivery method.
        /// </summary>
        /// <param name="mailMessage">The mail message to send.</param>
        /// <param name="deliveryMethod">
        /// A object that specifies the delivery method to use. <c>null</c> indicates that
        /// the default delivery method (as specified in config) will be used.
        /// </param>
        /// <returns>A task that completes once the mail message has been delivered.</returns>
        public static async Task SendAsync(this MailMessage mailMessage, DeliveryMethod deliveryMethod = null)
        {
            using (var smtpClient = GetSmtpClient(deliveryMethod))
            {
                await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Synchronously sends the specified mail message using the specified
        /// delivery method.
        /// </summary>
        /// <param name="mailMessage">The mail message to send.</param>
        /// <param name="deliveryMethod">
        /// A object that specifies the delivery method to use. <c>null</c> indicates that
        /// the default delivery method (as specified in config) will be used.
        /// </param>
        public static void Send(this MailMessage mailMessage, DeliveryMethod deliveryMethod = null)
        {
            using (var smtpClient = GetSmtpClient(deliveryMethod))
            {
                smtpClient.Send(mailMessage);
            }
        }

        private static SmtpClient GetSmtpClient(DeliveryMethod deliveryMethod)
        {
            var smtpClient = new SmtpClient();
            (deliveryMethod ?? DeliveryMethod.Default).ConfigureSmtpClient(smtpClient);
            return smtpClient;
        }
    }
}