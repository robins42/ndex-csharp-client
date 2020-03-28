using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// <para>baseUrl/admin</para>
    /// Documentations are based on NDEx 2.4 documentation
    /// </summary>
    public class Admin
    {
        private readonly NetworkConfiguration _network;
        
        internal Admin(NetworkConfiguration network)
        {
            _network = network;
        }
        
        /// <summary>
        /// <para>GET: /admin/status</para>
        /// Get Server Status
        /// <list type="bullet">
        /// <item><description>Get the current operational status of an NDEx server.</description></item>
        /// <item><description>Use this function to check if the NDEx server is running or stopped,
        /// as well as list important server properties.</description></item>
        /// <item><description>The format query parameter determines what properties are listed by this operation.</description></item>
        /// </list>
        /// <para>Authentication: Not required</para>
        /// </summary>
        /// <param name="format">Determines the server information to be returned.</param>
        public async Task<NDExStatus> GetStatus(AdminStatusFormat format = AdminStatusFormat.Full)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/admin/status");
            switch (format)
            {
                default:
                    restRequest.AddUrlSegment("format", "full");
                    break;
                case AdminStatusFormat.Short:
                    restRequest.AddUrlSegment("format", "short");
                    break;
            }
            
            return await _network.client.ExecuteAsync<NDExStatus>(restRequest);
        }
    }
}