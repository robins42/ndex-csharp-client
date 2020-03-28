using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// <para>baseUrl/group</para>
    /// Documentations are based on NDEx 2.4 documentation
    /// </summary>
    public class Group
    {
        private readonly NetworkConfiguration _network;
        
        internal Group(NetworkConfiguration network)
        {
            _network = network;
        }
        
        /// <summary>
        /// <para>POST: /group</para>
        /// Create a Group
        /// <para>Creates a new group owned by the authenticated user.</para>
        /// <para>A URL link associated with the newly created group is returned in the response payload upon
        /// successful completion.</para>
        /// <para>The group's UUID is a component of this URL and corresponds to the groupid parameter used in subsequent
        /// Get/Put/Delete group operations.</para>
        /// <para>Returns an error if
        /// <list type="bullet">
        /// <item><description>the groupName property is not specified or already in use</description></item>
        /// <item><description>the user is not authenticated</description></item>
        /// </list></para>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<RestResponse> Create(model.Group group)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/group");
            restRequest.SetContentBody(group);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /group/{groupId}</para>
        /// Get a Group
        /// <list type="bullet">
        /// <item><description>Retrieves the group specified by groupid (i.e. Group UUID).</description></item>
        /// <item><description>Returns an error if the group is not found.</description></item>
        /// </list>
        /// <para>Authentication: Not required</para>
        /// </summary>
        public async Task<model.Group> Get(Guid groupId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/group/" + groupId);
            return await _network.client.ExecuteAsync<model.Group>(restRequest);
        }
        
        /// <summary>
        /// <para>PUT: /group/{groupId}</para>
        /// Update a Group
        /// <para>Updates the group metadata associated with the groupid path parameter.</para>
        /// <para>Returns an error if:</para>
        /// <list type="bullet">
        /// <item><description>the group is not found</description></item>
        /// <item><description>the user is not the owner of the network set</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<RestResponse> Update(model.Group updatedGroup, Guid groupId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/group/" + groupId);
            restRequest.SetContentBody(updatedGroup);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>DELETE: /group/{groupId}</para>
        /// Delete a Group
        /// <para>Deletes the group specified by the groupid path parameter.</para>
        /// <para>Returns an error if:</para>
        /// <list type="bullet">
        /// <item><description>the group is not found</description></item>
        /// <item><description>the authenticated user does not have authorization to delete the group</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<RestResponse> Delete(Guid groupId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.DELETE, "/group/" + groupId);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /group/{groupId}/membership</para>
        /// Get Members of a Group
        /// <list type="bullet">
        /// <item><description>Retrieves members of the group associated with the groupid path parameter.</description></item>
        /// <item><description>The permissionFilter parameter  can filter which membership types are returned. All membership types are returned if
        /// this query param is omitted. Available values: GROUPADMIN, MEMBER</description></item>
        /// </list>
        /// <para>Authentication: Optional</para>
        /// </summary>
        public async Task<List<Membership>> GetGroupMembers(
            Guid groupId, Permissions? permissionFilter, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/group/" + groupId + "/membership");
            restRequest.AddUrlSegment("type", permissionFilter);
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount);
            return await _network.client.ExecuteAsync<List<Membership>>(restRequest);
        }
        
        /// <summary>
        /// <para>PUT: /group/{groupId}/membership</para>
        /// Add or Update a Group Member
        /// <para>Updates the group membership associated with the groupid path parameter.</para>
        /// <para>An error is returned if:</para>
        /// <list type="bullet">
        /// <item><description>any parameter is omitted</description></item>
        /// <item><description>the authenticated user does not have Admin permissions for the group</description></item>
        /// <item><description>the change would leave the group without an Admin member</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<RestResponse> AddOrUpdateGroupMember(Guid groupId, Guid userId, Permissions permission)
        {
            RestRequest restRequest = new RestRequest(RestMethod.PUT, "/group/" + groupId + "/membership");
            restRequest.AddUrlSegment("userid", userId);
            restRequest.AddUrlSegment("type", permission);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>DELETE: /group/{groupId}/membership</para>
        /// Remove a Group Member
        /// <para>Removes the group member associated with the userid query parameter.</para>
        /// <para>An error is returned if:</para>
        /// <list type="bullet">
        /// <item><description>the group does not exist</description></item>
        /// <item><description>the authenticated user is not authorized to edit this group</description></item>
        /// <item><description>removing the member would leave the group without an Admin member</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<RestResponse> RemoveGroupMember(Guid groupId, Guid memberId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.DELETE, "/group/" + groupId + "/membership");
            restRequest.AddUrlSegment("userid", memberId);
            return await _network.client.ExecuteAsync(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /group/{groupId}/permission</para>
        /// Get Group Permission for a Specific Network
        /// <list type="bullet">
        /// <item><description>Returns the explicit permission the specified groupid has on the specified networkid,
        /// which is either READ or WRITE.</description></item>
        /// <item><description>Authentication is manditory to ensure only members of a group can obtain group-network permissions.</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<Dictionary<Guid, Permissions>> GetPermissionsOfGroup(Guid groupId, Guid networkId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/group/" + groupId + "/permission");
            restRequest.AddUrlSegment("networkid", networkId);
            return await _network.client.ExecuteAsync<Dictionary<Guid, Permissions>>(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /group/{groupId}/permission</para>
        /// Get Network Permissions of a Group
        /// <list type="bullet">
        /// <item><description>Returns network permissions for the specified group.</description></item>
        /// <item><description>The returned dictionary object contains key/value pairs, where the key is the network UUID and the value is
        /// the corresponding permission.</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<List<Membership>> GetPermissionsOfGroupMembers(
            Guid groupId, Permissions? permissions, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/group/" + groupId + "/membership");
            restRequest.AddUrlSegment("permission", permissions);
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount);
            return await _network.client.ExecuteAsync<List<Membership>>(restRequest);
        }
    }
}