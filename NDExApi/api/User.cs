using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// <para>baseUrl/user</para>
    /// Creating and updating a user is not part of the public NDEx API, you have to do it manually.
    /// </summary>
    public class User
    {
        private readonly NetworkConfiguration _network;
        
        internal User(NetworkConfiguration network)
        {
            _network = network;
        }
        
        /// <summary>
        /// <para>GET: /user/{userid}/membership</para>
        /// </summary>
        public async Task<Permissions> GetMembershipOfSpecificGroup(Guid userId, Guid groupId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/user/" + userId + "/membership");
            restRequest.AddUrlSegment("groupid", groupId);
            Dictionary<Guid, Permissions> permissions = 
                await _network.client.ExecuteAsync<Dictionary<Guid, Permissions>>(restRequest);
            
            // Only 1 permission with these parameters
            return permissions[groupId];
        }
        
        /// <summary>
        /// <para>GET: /user/{userid}/membership</para>
        /// </summary>
        public async Task<Dictionary<Guid, Permissions>> GetMembershipOfAllGroups(
            Guid userId, Permissions membershipType, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/user/" + userId + "/membership");
            restRequest.AddUrlSegment("type", membershipType);
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount);
            return await _network.client.ExecuteAsync<Dictionary<Guid, Permissions>>(restRequest);
        }

        /// <summary>
        /// <para>GET: /user/{userid}/networksets</para>
        /// </summary>
        public async Task<List<model.NetworkSet>> GetNetworkSets(
            Guid userId, bool summaryOnly = false, bool showcasedOnly = false, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/user/" + userId + "/networksets");
            restRequest.AddUrlSegment("offset", startIndex);
            restRequest.AddUrlSegment("limit", amount);
            restRequest.AddUrlSegment("summary", summaryOnly);
            restRequest.AddUrlSegment("showcase", showcasedOnly);
            return await _network.client.ExecuteAsync<List<model.NetworkSet>>(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /user/{userid}/networksummary</para>
        /// </summary>
        public async Task<List<NetworkSummary>> GetNetworkSummaries(Guid userId, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/user/" + userId + "/networksummary");
            restRequest.AddUrlSegment("offset", startIndex);
            restRequest.AddUrlSegment("limit", amount);
            return await _network.client.ExecuteAsync<List<NetworkSummary>>(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /user/{userid}/permission</para>
        /// </summary>
        public async Task<Permissions> GetNetworkPermissionInfo(Guid userId, Guid networkId, bool directOnly = false)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/user/" + userId + "/permission");
            restRequest.AddUrlSegment("networkid", networkId);
            restRequest.AddUrlSegment("directonly", directOnly);
            Dictionary<Guid, Permissions> permissions = 
                await _network.client.ExecuteAsync<Dictionary<Guid, Permissions>>(restRequest);
            
            // Only 1 permission?
            return permissions[networkId];
        }
    }
}

