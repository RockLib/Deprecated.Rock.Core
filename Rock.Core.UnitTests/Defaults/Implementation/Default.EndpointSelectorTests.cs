using Rock.Net;

namespace DefaultHelperTests.Implementation
{
    internal class Default_EndpointSelectorTests : DefaultTestBase<IEndpointSelector, EndpointSelector>
    {
        protected override string PropertyName
        {
            get { return "EndpointSelector"; }
        }
    }
}