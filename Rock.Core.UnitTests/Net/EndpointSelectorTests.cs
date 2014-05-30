using System.Net;
using Moq;
using NUnit.Framework;
using Rock.Net;

// ReSharper disable once CheckNamespace
namespace EndpointSelectorTests
{
    public class TheGetFirstToRespondMethod
    {
        [Test]
        public void ReturnsTheFirstEndpointToRespond()
        {
            const string nowhere = "http://nowhere.com";
            const string google = "http://google.com";

            var mockEndpointDetector = new Mock<IEndpointDetector>();

            mockEndpointDetector
                .Setup(m => m.GetEndpointStatus(It.IsAny<int>(), It.Is<string>(x => x != google)))
                .ReturnsAsync(new EndpointStatus(false, nowhere));
            
            mockEndpointDetector
                .Setup(m => m.GetEndpointStatus(It.IsAny<int>(), It.Is<string>(x => x == google)))
                .ReturnsAsync(new EndpointStatus(true, google, HttpStatusCode.OK));

            var endpointSelector = new EndpointSelector(mockEndpointDetector.Object);

            var address = endpointSelector.GetFirstToRespond(nowhere, google).Result;

            Assert.That(address, Is.EqualTo(google));
        }

        [Test]
        public void ReturnsNullIfNoEndpointsRespond()
        {
            const string nowhere = "http://nowhere.com";

            var mockEndpointDetector = new Mock<IEndpointDetector>();

            mockEndpointDetector
                .Setup(m => m.GetEndpointStatus(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new EndpointStatus(false, nowhere));

            var endpointSelector = new EndpointSelector(mockEndpointDetector.Object);

            var address = endpointSelector.GetFirstToRespond(nowhere, nowhere).Result;

            Assert.That(address, Is.Null);
        }
    }
}