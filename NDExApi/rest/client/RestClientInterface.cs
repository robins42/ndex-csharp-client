using System.Threading.Tasks;

namespace NDExApi.rest.client
{
    internal interface RestClientInterface
    {
        Task<T> ExecuteAsync<T>(RestRequest rest);

        Task<RestResponse> ExecuteAsync(RestRequest rest);

        /// This JSON header breaks other calls for some reason so the JSON header is only used temporarily 
        Task<T> ExecuteForJsonAsync<T>(RestRequest restRequest);
    }
}