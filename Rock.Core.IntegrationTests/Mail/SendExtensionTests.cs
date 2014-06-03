using System;
using System.IO;
using System.Net.Mail;
using NUnit.Framework;
using Rock.Core.IntegrationTests.Mail;
using Rock.Mail;

// ReSharper disable once CheckNamespace
namespace SendExtensionsTests
{
    public class SendExtensionTests
    {
        public class TheSendExtensionMethod
        {
            private MailMessage _mailMessage;

            [SetUp]
            public void Setup()
            {
                _mailMessage = new MailMessage(
                    "brian.friesen@gmail.com",
                    "brianfriesen@quickenloans.com",
                    "Hello, World!",
                    "Such hello. Very world.");
            }

            [Test]
            public void UsesTheDeliveryMethodSpecifiedInConfigurationWhenNoDeliveryMethodParameterIsSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7777)) // port 7777 defined in app.config
                {
                    _mailMessage.Send().Wait();

                    mailData = server.GetMailData().Result;
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public void UsesTheDeliveryMethodSpecifiedInConfigurationWhenTheDefaultDeliveryMethodIsSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7777)) // port 7777 defined in app.config
                {
                    _mailMessage.Send(DeliveryMethod.Default).Wait();

                    mailData = server.GetMailData().Result;
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public void UsesTheNetworkDeliveryMethodWhenSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7778))
                {
                    _mailMessage.Send(DeliveryMethod.Network("localhost", 7778)).Wait();

                    mailData = server.GetMailData().Result;
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public void UsesTheSpecifiedPickupDirectoryDeliveryMethodWhenSpecified()
            {
                var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(dir);

                Assert.That(Directory.GetFiles(dir), Is.Empty);

                _mailMessage.Send(DeliveryMethod.SpecifiedPickupDirectory(dir)).Wait();

                Assert.That(Directory.GetFiles(dir), Is.Not.Empty);

                // Cleanup
                Directory.Delete(dir, true);
            }
        }
    }
}