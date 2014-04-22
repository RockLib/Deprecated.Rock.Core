using System.Net.Mail;

namespace Rock.Mail
{
    public interface IDeliveryMethod
    {
        void ConfigureSmtpClient(SmtpClient smtpClient);
    }
}