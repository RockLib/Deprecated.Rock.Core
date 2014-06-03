using System.Net.Mail;
using NUnit.Framework;
using Rock.Mail;

// ReSharper disable once CheckNamespace
namespace DeliveryMethodTests
{
    public class TheDefaultDeliveryMethod
    {
        [Test]
        public void DoesNotChangeTheDeliveryMethodOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis,
            };

            var deliveryMethod = DeliveryMethod.Default;
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.DeliveryMethod, Is.EqualTo(SmtpDeliveryMethod.PickupDirectoryFromIis));
        }

        [Test]
        public void DoesNotChangeTheHostOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                Host = "foobar",
            };

            var deliveryMethod = DeliveryMethod.Default;
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.Host, Is.EqualTo("foobar"));
        }

        [Test]
        public void DoesNotChangeThePortOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                Port = 1111
            };

            var deliveryMethod = DeliveryMethod.Default;
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.Port, Is.EqualTo(1111));
        }

        [Test]
        public void DoesNotChangeThePickupDirectoryLocationOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                PickupDirectoryLocation = "foobar"
            };

            var deliveryMethod = DeliveryMethod.Default;
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.PickupDirectoryLocation, Is.EqualTo("foobar"));
        }
    }


    public class TheNetworkDeliveryMethod
    {
        [Test]
        public void ChangesTheDeliveryMethodOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis,
            };

            var deliveryMethod = DeliveryMethod.Network("localhost", 7777);
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.DeliveryMethod, Is.EqualTo(SmtpDeliveryMethod.Network));
        }

        [Test]
        public void ChangesTheHostOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                Host = "foobar"
            };

            var deliveryMethod = DeliveryMethod.Network("localhost", 7777);
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.Host, Is.EqualTo("localhost"));
        }

        [Test]
        public void ChangesThePortOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                Port = 1111
            };

            var deliveryMethod = DeliveryMethod.Network("localhost", 7777);
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.Port, Is.EqualTo(7777));
        }

        [Test]
        public void DoesNotChangeThePickupDirectoryLocationOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                PickupDirectoryLocation = "foobar"
            };

            var deliveryMethod = DeliveryMethod.Network("localhost", 7777);
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.PickupDirectoryLocation, Is.EqualTo("foobar"));
        }
    }

    public class TheSpecifiedPickupDirectoryDeliveryMethod
    {
        [Test]
        public void ChangesTheDeliveryMethodOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis,
            };

            var deliveryMethod = DeliveryMethod.SpecifiedPickupDirectory(@"c:\temp");
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.DeliveryMethod, Is.EqualTo(SmtpDeliveryMethod.SpecifiedPickupDirectory));
        }

        [Test]
        public void DoesNotChangeTheHostOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                Host = "foobar"
            };

            var deliveryMethod = DeliveryMethod.SpecifiedPickupDirectory(@"c:\temp");
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.Host, Is.EqualTo("foobar"));
        }

        [Test]
        public void DoesNotChangeThePortOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                Port = 1111
            };

            var deliveryMethod = DeliveryMethod.SpecifiedPickupDirectory(@"c:\temp");
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.Port, Is.EqualTo(1111));
        }

        [Test]
        public void DoesNotChangeThePickupDirectoryLocationOfAnSmtpClient()
        {
            var client = new SmtpClient
            {
                PickupDirectoryLocation = "foobar"
            };

            var deliveryMethod = DeliveryMethod.SpecifiedPickupDirectory(@"c:\temp");
            deliveryMethod.ConfigureSmtpClient(client);

            Assert.That(client.PickupDirectoryLocation, Is.EqualTo(@"c:\temp"));
        }
    }
}