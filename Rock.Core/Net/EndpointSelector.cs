using System.Linq;
using System.Threading.Tasks;

namespace Rock.Net
{
    public class EndpointSelector : IEndpointSelector
    {
        private readonly IEndpointDetector _endpointDetector;

        public EndpointSelector(IEndpointDetector endpointDetector)
        {
            _endpointDetector = endpointDetector;
        }

        public async Task<string> GetFirstToRespond(int timeoutSeconds, params string[] endpoints)
        {
            var tasks = endpoints.Select(endpoint => _endpointDetector.GetEndpointStatus(timeoutSeconds, endpoint)).ToList();

            while (tasks.Count > 0)
            {
                // Whenever any task finishes...
                var task = await Task.WhenAny(tasks).ConfigureAwait(false);

                // Remove that task from the list so the while loop can terminate if no endpoints are successful.
                tasks.Remove(task);

                switch (task.Status)
                {
                    // If the task ran to completion...
                    case TaskStatus.RanToCompletion:
                    {
                        // ...and the server actually responded...
                        if (task.Result.Success)
                        {
                            // ...then return that endpoint.
                            return task.Result.Endpoint;
                        }

                        break;
                    }
                }
            }

            return null;
        }
    }
}