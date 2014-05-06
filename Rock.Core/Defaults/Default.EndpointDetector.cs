using System;
using Rock.Net;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IEndpointDetector> _defaultEndpointDetector = new Default<IEndpointDetector>(() => new EndpointDetector());

        public static IEndpointDetector EndpointDetector
        {
            get { return _defaultEndpointDetector.Current; }
        }

        public static void SetEndpointDetector(Func<IEndpointDetector> getEndpointDetectorInstance)
        {
            _defaultEndpointDetector.SetCurrent(getEndpointDetectorInstance);
        }
    }
}