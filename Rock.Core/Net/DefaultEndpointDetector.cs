using Rock.Immutable;

namespace Rock.Net
{
    public static class DefaultEndpointDetector
    {
        private static readonly Semimutable<IEndpointDetector> _endpointDetector = new Semimutable<IEndpointDetector>(GetDefault);

        public static IEndpointDetector Current
        {
            get { return _endpointDetector.Value; }
        }

        public static void SetCurrent(IEndpointDetector endpointDetector)
        {
            _endpointDetector.Value = endpointDetector;
        }

        private static IEndpointDetector GetDefault()
        {
            return new EndpointDetector();
        }
    }
}