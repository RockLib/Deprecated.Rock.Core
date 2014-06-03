using System.Net.Mail;

namespace Rock.Mail
{
    /// <summary>
    /// Defines three methods for delivering mail: <see cref="Default"/>, <see cref="Network"/>, and 
    /// <see cref="SpecifiedPickupDirectory"/>.
    /// </summary>
    public abstract partial class DeliveryMethod
    {
        /// <summary>
        /// When overridden in a derived class, sets various properties of <paramref name="smtpClient"/>.
        /// </summary>
        /// <param name="smtpClient">The <see cref="SmtpClient"/> whose property values will be customized.</param>
        public abstract void ConfigureSmtpClient(SmtpClient smtpClient);

        /// <summary>
        /// Gets a delivery method that is configured by an application's web.config or app.config.
        /// </summary>
        public static DeliveryMethod Default
        {
            get { return DefaultDeliveryMethod.Instance; }
        }

        /// <summary>
        /// Gets a delivery method for sending mail to the specified host using the specified port.
        /// </summary>
        /// <param name="host">The host to send mail to.</param>
        /// <param name="port">The port on which to send mail. Optional.</param>
        /// <returns>A <see cref="NetworkDeliveryMethod"/> with the specified host and port.</returns>
        public static DeliveryMethod Network(string host, int port = 25)
        {
            return new NetworkDeliveryMethod(host, port);
        }

        /// <summary>
        /// Gets a delivery method for saving mail messages to a target directory, accessable from the local machine.
        /// </summary>
        /// <param name="pickupDirectoryLocation">The directory in which to save mail messages.</param>
        /// <returns>
        /// A <see cref="SpecifiedPickupDirectoryDeliveryMethod"/>delivery method with the specified target directory.
        /// </returns>
        public static DeliveryMethod SpecifiedPickupDirectory(string pickupDirectoryLocation)
        {
            return new SpecifiedPickupDirectoryDeliveryMethod(pickupDirectoryLocation);
        }
    }
}