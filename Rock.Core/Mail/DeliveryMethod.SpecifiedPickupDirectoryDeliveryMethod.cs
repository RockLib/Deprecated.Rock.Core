using System.Net.Mail;

namespace Rock.Mail
{
    public partial class DeliveryMethod
    {
        private class SpecifiedPickupDirectoryDeliveryMethod : DeliveryMethod
        {
            private readonly string _pickupDirectoryLocation;

            public SpecifiedPickupDirectoryDeliveryMethod(string pickupDirectoryLocation)
            {
                _pickupDirectoryLocation = pickupDirectoryLocation;
            }

            internal override void ConfigureSmtpClient(SmtpClient smtpClient)
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtpClient.PickupDirectoryLocation = _pickupDirectoryLocation;
            }
        }
    }
}