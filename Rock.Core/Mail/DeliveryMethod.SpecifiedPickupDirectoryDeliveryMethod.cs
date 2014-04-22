using System.Net.Mail;

namespace Rock.Mail
{
    public static partial class DeliveryMethod
    {
        private class SpecifiedPickupDirectoryDeliveryMethod : IDeliveryMethod
        {
            private readonly string _pickupDirectoryLocation;

            public SpecifiedPickupDirectoryDeliveryMethod(string pickupDirectoryLocation)
            {
                _pickupDirectoryLocation = pickupDirectoryLocation;
            }

            public void ConfigureSmtpClient(SmtpClient smtpClient)
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtpClient.PickupDirectoryLocation = _pickupDirectoryLocation;
            }
        }
    }
}