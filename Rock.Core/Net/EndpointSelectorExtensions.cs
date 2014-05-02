using System.Threading.Tasks;

namespace Rock.Net
{
    public static class EndpointSelectorExtensions
    {
        public static Task<string> GetFirstToRespond(this IEndpointSelector endpointSelector, params string[] endpoints)
        {
            return endpointSelector.GetFirstToRespond(15, endpoints);
        }
    }
}