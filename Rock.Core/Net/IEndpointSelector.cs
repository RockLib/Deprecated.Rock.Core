using System.Threading.Tasks;

namespace Rock.Net
{
    public interface IEndpointSelector
    {
        Task<string> GetFirstToRespond(int timeoutSeconds, params string[] endpoints);
    }
}