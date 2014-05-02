using System.Threading.Tasks;

namespace Rock.Net
{
    public interface IEndpointDetector
    {
        Task<EndpointStatus> GetEndpointStatus(int timeoutSeconds, string endpoint);
    }
}