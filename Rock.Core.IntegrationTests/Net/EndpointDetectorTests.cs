using NUnit.Framework;
using Rock.Net;

namespace EndpointDetectorTests
{
    public class TheGetEndpointStatusMethod
    {
        [Test]
        public void ReturnsSuccessfulEndpointStatusWhenTheServerIsReachable()
        {
            const string google = "http://google.com";

            var detector = new EndpointDetector();

            var result = detector.GetEndpointStatus(15, google).Result;

            Assert.That(result.Endpoint, Is.EqualTo(google));
            Assert.That(result.Success, Is.True);
        }

        [Test]
        [CategoryAttribute("Failing Tests")] // TODO: implement https endpoint detection.
        public void WorksWithHttps()
        {
            const string google = "https://google.com";

            var detector = new EndpointDetector();

            var result = detector.GetEndpointStatus(15, google).Result;

            Assert.That(result.Endpoint, Is.EqualTo(google));
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void ReturnsUnsuccessfulEndpointStatusWhenTheServerDoesNotExist()
        {
            const string google = "http://asdvasDFJKLASHUERTLAEGKS.com";

            var detector = new EndpointDetector();

            var result = detector.GetEndpointStatus(15, google).Result;
            
            Assert.That(result.Endpoint, Is.EqualTo(google));
            Assert.That(result.Success, Is.False);
        }
    }
}