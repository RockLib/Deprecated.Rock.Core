using System;
using Rock.Net;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IEndpointSelector> _defaultEndpointSelector = new Default<IEndpointSelector>(() => new EndpointSelector(_defaultEndpointDetector.DefaultInstance));

        public static IEndpointSelector EndpointSelector
        {
            get { return _defaultEndpointSelector.Current; }
        }

        public static void SetEndpointSelector(Func<IEndpointSelector> getEndpointSelectorInstance)
        {
            _defaultEndpointSelector.SetCurrent(getEndpointSelectorInstance);
        }
    }
}