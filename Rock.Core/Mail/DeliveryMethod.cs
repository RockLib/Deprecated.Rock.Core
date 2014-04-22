namespace Rock.Mail
{
    public static partial class DeliveryMethod
    {
        public static IDeliveryMethod Default
        {
            get { return DefaultDeliveryMethod.Instance; }
        }

        public static IDeliveryMethod Network(string host, int port = 25)
        {
            return new NetworkDeliveryMethod(host, port);
        }

        public static IDeliveryMethod SpecifiedPickupDirectory(string pickupDirectoryLocation)
        {
            return new SpecifiedPickupDirectoryDeliveryMethod(pickupDirectoryLocation);
        }
    }
}