using Rock.Net;

namespace DefaultHelperTests.Implementation
{
    public class Default_EndpointDetectorTests : DefaultTestBase<IEndpointDetector, EndpointDetector>
    {
        protected override string PropertyName
        {
            get { return "EndpointDetector"; }
        }
    }
}