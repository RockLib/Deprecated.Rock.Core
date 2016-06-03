using System.Net.Mail;
using System.Threading.Tasks;

namespace Rock.Mail
{
    public static class SendExtension
    {
        public static async Task SendAsync(this MailMessage mailMessage, DeliveryMethod deliveryMethod = null)
        {
            using (var smtpClient = new SmtpClient())
            {
                (deliveryMethod ?? DeliveryMethod.Default).ConfigureSmtpClient(smtpClient);
                await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
            }
        }
    }
}