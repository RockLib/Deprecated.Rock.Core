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
        public class TheSendAsyncExtensionMethod
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
            public async void UsesTheDeliveryMethodSpecifiedInConfigurationWhenNoDeliveryMethodParameterIsSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7777)) // port 7777 defined in app.config
                {
                    await _mailMessage.SendAsync();

                    mailData = await server.GetMailDataAsync();
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public async void UsesTheDeliveryMethodSpecifiedInConfigurationWhenTheDefaultDeliveryMethodIsSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7777)) // port 7777 defined in app.config
                {
                    await _mailMessage.SendAsync(DeliveryMethod.Default);

                    mailData = await server.GetMailDataAsync();
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public async void UsesTheNetworkDeliveryMethodWhenSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7778))
                {
                    await _mailMessage.SendAsync(DeliveryMethod.Network("localhost", 7778));

                    mailData = await server.GetMailDataAsync();
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public async void UsesTheSpecifiedPickupDirectoryDeliveryMethodWhenSpecified()
            {
                var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(dir);

                Assume.That(Directory.GetFiles(dir), Is.Empty);

                await _mailMessage.SendAsync(DeliveryMethod.SpecifiedPickupDirectory(dir));

                Assert.That(Directory.GetFiles(dir), Is.Not.Empty);

                // Cleanup
                Directory.Delete(dir, true);
            }
        }

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
            public async void UsesTheDeliveryMethodSpecifiedInConfigurationWhenNoDeliveryMethodParameterIsSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7777)) // port 7777 defined in app.config
                {
                    _mailMessage.Send();

                    mailData = await server.GetMailDataAsync();
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public async void UsesTheDeliveryMethodSpecifiedInConfigurationWhenTheDefaultDeliveryMethodIsSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7777)) // port 7777 defined in app.config
                {
                    _mailMessage.Send(DeliveryMethod.Default);

                    mailData = await server.GetMailDataAsync();
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public async void UsesTheNetworkDeliveryMethodWhenSpecified()
            {
                string mailData;

                using (var server = LocalMailServer.StartNew(7778))
                {
                    _mailMessage.Send(DeliveryMethod.Network("localhost", 7778));

                    mailData = await server.GetMailDataAsync();
                }

                Assert.That(mailData.Contains("Hello, World!"));
            }

            [Test]
            public void UsesTheSpecifiedPickupDirectoryDeliveryMethodWhenSpecified()
            {
                var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(dir);

                Assume.That(Directory.GetFiles(dir), Is.Empty);

                _mailMessage.Send(DeliveryMethod.SpecifiedPickupDirectory(dir));

                Assert.That(Directory.GetFiles(dir), Is.Not.Empty);

                // Cleanup
                Directory.Delete(dir, true);
            }
        }
    }
}