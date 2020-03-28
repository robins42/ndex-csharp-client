using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// baseUrl/network
    /// </summary>
    public class Network
    {
        private readonly NetworkConfiguration _network;
        
        internal Network(NetworkConfiguration network)
        {
            _network = network;
        }
        
        /// <summary>
        /// <para>POST: /network</para>
        /// </summary>
        public async Task<RestResponse> Create(Visibility? visibility, string cxJson)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/network");
            restRequest.AddUrlSegment("visibility", visibility);                
            restRequest.SetContentBody(cxJson);
            
            return await _network.client.ExecuteAsync(restRequest);
        }

        /// <summary>
        /// <para>GET: /network/{networkid}</para>
        /// </summary>
        public async Task<string> GetComplete(Guid networkId, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId);
            restRequest.AddUrlSegment("accesskey", accessKey);
            RestResponse response = await _network.client.ExecuteAsync(restRequest);
            return response.json;
        }

        /// <summary>
        /// <para>PUT: /network/{networkid}</para>
        /// </summary>
        public async Task<RestResponse> Update(Guid networkId, string cxJson)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId);
            restRequest.SetContentBody(cxJson);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>DELETE: /network/{networkid}</para>
        /// </summary>
        public async Task<RestResponse> Delete(Guid networkId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.DELETE, "/network/" + networkId);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /network/{networkid}/accesskey</para>
        /// </summary>
        public async Task<string> GetAccessKey(Guid networkId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId + "/accesskey");
            Dictionary<string, string> accessKeyMap = await _network.client.ExecuteAsync<Dictionary<string, string>>(restRequest);
            
            // Map only has 1 entry as of NDEx 2.4 or the dictionary is null if access keys are disabled
            return accessKeyMap != null ? accessKeyMap["accessKey"] : null;
        }
        
        /// <summary>
        /// <para>PUT: /network/{networkid}/accesskey</para>
        /// </summary>
        public async Task<RestResponse> DisableEnableAccessKey(Guid networkId, AccessKeyAction action)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/accesskey");
            restRequest.AddUrlSegment("action", action);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /network/{networkid}/aspect</para>
        /// </summary>
        public async Task<MetadataCollection> GetCxMetadataCollection(Guid networkId, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId + "/aspect");
            restRequest.AddUrlSegment("accesskey", accessKey);
            return await _network.client.ExecuteAsync<MetadataCollection>(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /network/{networkid}/aspect/{aspectname}</para>
        /// </summary>
        public async Task<List<Dictionary<string, object>>> GetAspectElements(Guid networkId, string aspectName, int limit = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId + "/aspect/" + aspectName);
            restRequest.AddUrlSegment("limit", limit);                
            return await _network.client.ExecuteAsync<List<Dictionary<string, object>>>(restRequest);
        }
                
        /// <summary>
        /// <para>GET: /network/{networkid}/aspect/{aspectname}/metadata</para>
        /// </summary>
        public async Task<MetadataElement> GetAspectMetadata(Guid networkId, string aspectName)
        {
            // This call seems to only work if the client specifies that it wants a JSON response.
            
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId + "/aspect/" + aspectName + "/metadata");
            MetadataElement metadata = await _network.client.ExecuteForJsonAsync<MetadataElement>(restRequest);
            return metadata;
        }

        /// <summary>
        /// <para>PUT: /network/{networkid}/aspects</para>
        /// </summary>
        public async Task<RestResponse> UpdateAspects(Guid networkId, string cxJson)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/aspects");
            restRequest.SetContentBody(cxJson);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>POST: /network/{networkid}/copy</para>
        /// </summary>
        public async Task<Guid> Clone(Guid sourceNetworkId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/network/" + sourceNetworkId + "/copy");
            RestResponse response = await _network.client.ExecuteAsync(restRequest);
            string completeUrl = response.json;
            string idFromUrl = completeUrl.Substring(completeUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);
            return new Guid(idFromUrl);
        }
        
        /// <summary>
        /// <para>PUT: /network/{networkid}/profile</para>
        /// </summary>
        public async Task<RestResponse> UpdateProfile(Guid networkId, NetworkSummary partialSummary)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/profile");
            restRequest.SetContentBody(partialSummary);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>PUT: /network/{networkid}/properties</para>
        /// </summary>
        public async Task<RestResponse> SetProperties(Guid networkId, List<NDExPropertyValuePair> properties)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/properties");
            restRequest.SetContentBody(properties);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /network/{networkid}/provenance</para>
        /// </summary>
        public async Task<ProvenanceEntity> GetProvenance(Guid networkId, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId + "/provenance");
            restRequest.AddUrlSegment("accesskey", accessKey);
            return await _network.client.ExecuteAsync<ProvenanceEntity>(restRequest);
        }
                
        /// <summary>
        /// <para>PUT: /network/{networkid}/provenance</para>
        /// </summary>
        public async Task<RestResponse> SetProvenance(Guid networkId, ProvenanceEntity provenance)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/provenance");
            restRequest.SetContentBody(provenance);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /network/{networkid}/sample</para>
        /// </summary>
        public async Task<string> GetSample(Guid networkId, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId + "/sample");
            restRequest.AddUrlSegment("accesskey", accessKey);
            RestResponse response = await _network.client.ExecuteAsync(restRequest);
            return response.json;
        }
                
        /// <summary>
        /// <para>PUT: /network/{networkid}/sample</para>
        /// </summary>
        public async Task<RestResponse> SetSample(Guid networkId, string cxString)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/sample");
            restRequest.SetContentBody(cxString);
            return await _network.client.ExecuteAsync(restRequest);
        }
                        
        /// <summary>
        /// <para>GET: /network/{networkid}/summary</para>
        /// </summary>
        public async Task<NetworkSummary> GetSummary(Guid networkId, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/network/" + networkId + "/summary");
            restRequest.AddUrlSegment("accesskey", accessKey);
            return await _network.client.ExecuteAsync<NetworkSummary>(restRequest);
        }
        
        /// <summary>
        /// <para>PUT: /network/{networkid}/summary</para>
        /// </summary>
        public async Task<RestResponse> UpdateSummary(Guid networkId, NetworkSummary summary)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/summary");
            restRequest.SetContentBody(summary);
            return await _network.client.ExecuteAsync(restRequest);
        }
                
        /// <summary>
        /// <para>PUT: /network/{networkid}/systemproperty</para>
        /// </summary>
        public async Task<RestResponse> SetFlag(Guid networkId, NetworkSystemProperties properties)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/network/" + networkId + "/systemproperty");
            restRequest.SetContentBody(properties);
            return await _network.client.ExecuteAsync(restRequest);
        }
    }
}