using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// baseUrl/networkset
    /// </summary>
    public class NetworkSet
    {
        private readonly NetworkConfiguration _network;
        
        internal NetworkSet(NetworkConfiguration network)
        {
            _network = network;
        }
        
        /// <summary>
        /// <para>POST: /networkset</para>
        /// </summary>
        public async Task<RestResponse> Create(model.NetworkSet newNetworkSet)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/networkset");
            restRequest.SetContentBody(newNetworkSet);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /networkset/{networksetid}</para>
        /// </summary>
        public async Task<model.NetworkSet> Get(Guid networkSetId, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/networkset/" + networkSetId);
            restRequest.AddUrlSegment("accesskey", accessKey);
            return await _network.client.ExecuteAsync<model.NetworkSet>(restRequest);
        }
        
        /// <summary>
        /// <para>PUT: /networkset/{networksetid}</para>
        /// </summary>
        public async Task<RestResponse> Update(Guid networkSetId, model.NetworkSet newNetworkSet)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/networkset/" + networkSetId);
            restRequest.SetContentBody(newNetworkSet);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>DELETE: /networkset/{networksetid}</para>
        /// </summary>
        public async Task<RestResponse> Delete(Guid networkSetId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.DELETE, "/networkset/" + networkSetId);
            return await _network.client.ExecuteAsync(restRequest);
        }

        /// <summary>
        /// <para>POST: /networkset/{networksetid}/members</para>
        /// </summary>
        public async Task<RestResponse> AddNetworks(Guid networkSetId, HashSet<Guid> networkIds)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/networkset/" + networkSetId + "/members");
            restRequest.SetContentBody(networkIds);
            return await _network.client.ExecuteAsync(restRequest);
        }

        /// <summary>
        /// <para>DELETE: /networkset/{networksetid}/members</para>
        /// </summary>
        public async Task<RestResponse> RemoveNetworks(Guid networkSetId, HashSet<Guid> networkIds)
        {
            RestRequest restRequest = new RestRequest(RestMethod.DELETE, "/networkset/" + networkSetId + "/members");
            restRequest.SetContentBody(networkIds);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>PUT: /networkset/{networksetid}/systemproperty</para>
        /// </summary>
        public async Task<RestResponse> SetFlag(Guid networkSetId, NetworkSetSystemProperties properties)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/networkset/" + networkSetId + "/systemproperty");
            restRequest.SetContentBody(properties);
            return await _network.client.ExecuteAsync(restRequest);
        }
    }
}

