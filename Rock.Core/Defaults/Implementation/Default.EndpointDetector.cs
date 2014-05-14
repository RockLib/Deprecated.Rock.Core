using System;
using Rock.Net;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IEndpointDetector> _endpointDetector = new DefaultHelper<IEndpointDetector>(() => new EndpointDetector());

        public static IEndpointDetector EndpointDetector
        {
            get { return _endpointDetector.Current; }
        }

        public static IEndpointDetector DefaultEndpointDetector
        {
            get { return _endpointDetector.DefaultInstance; }
        }

        public static void SetEndpointDetector(Func<IEndpointDetector> getEndpointDetectorInstance)
        {
            _endpointDetector.SetCurrent(getEndpointDetectorInstance);
        }
    }
}