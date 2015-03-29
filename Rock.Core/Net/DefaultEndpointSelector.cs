using Rock.Immutable;

namespace Rock.Net
{
    public static class DefaultEndpointSelector
    {
        private static readonly Semimutable<IEndpointSelector> _endpointSelector = new Semimutable<IEndpointSelector>(GetDefault);

        public static IEndpointSelector Current
        {
            get { return _endpointSelector.Value; }
        }

        public static void SetCurrent(IEndpointSelector endpointSelector)
        {
            _endpointSelector.Value = endpointSelector;
        }

        private static IEndpointSelector GetDefault()
        {
            return new EndpointSelector(
                DefaultEndpointDetector.Current as EndpointDetector
                ?? new EndpointDetector());
        }
    }
}