using System;
using Rock.Net;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IEndpointSelector> _endpointSelector = new DefaultHelper<IEndpointSelector>(() => new EndpointSelector(_endpointDetector.DefaultInstance));

        public static IEndpointSelector EndpointSelector
        {
            get { return _endpointSelector.Current; }    
        }

        public static IEndpointSelector DefaultEndpointSelector
        {
            get { return _endpointSelector.DefaultInstance; }    
        }

        public static void SetEndpointSelector(Func<IEndpointSelector> getEndpointSelectorInstance)
        {
            _endpointSelector.SetCurrent(getEndpointSelectorInstance);
        }
    }
}