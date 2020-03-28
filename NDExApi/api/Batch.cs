using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// <para>baseUrl/batch</para>
    /// Documentations are based on NDEx 2.4 documentation
    /// </summary>
    public class Batch
    {
        private readonly NetworkConfiguration rest;
        
        internal Batch(NetworkConfiguration rest)
        {
            this.rest = rest;
        }
        
        /// <summary>
        /// <para>POST: /batch/group</para>
        /// Get Groups by UUIDs
        /// <list type="bullet">
        /// <item><description>Returns an array of Group objects.</description></item>
        /// <item><description>The HTTP request payload contains an array of UUIDs which identify which Group objects to fetch.</description></item>
        /// <item><description>No more than 2000 UUIDs are returned.</description></item>
        /// </list>
        /// <para>Authentication: Not required</para>
        /// </summary>
        public async Task<List<model.Group>> GetGroupsByIds(HashSet<Guid> ids)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/batch/group");
            restRequest.SetContentBody(ids);
            return await rest.client.ExecuteAsync<List<model.Group>>(restRequest);
        }
        
        /// <summary>
        /// <para>POST: /batch/network/export</para>
        /// Export Networks
        /// <list type="bullet">
        /// <item><description>Creates network export tasks for the set of networks specified by the networkids property
        /// in the network export request payload.</description></item>
        /// <item><description>The format which to export is specified using the exportFormat property in the request payload.</description></item>
        /// <item><description>The /admin/status?format=full function can be used to get a complete list of
        /// importers/exporters the server supports.</description></item>
        /// <item><description>Returns a dictionary object which maps Network UUIDs to ExportTaskIDs. The taskId can then be used to download the exported file.
        /// Refer to Task.DownloadExportedFile()</description></item>
        /// <item><description>No more than 1000 UUIDs are returned.</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<Dictionary<Guid, Guid>> ExportNetworks(NetworkExportRequest parameters)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/batch/network/export");
            restRequest.SetContentBody(parameters);
            return await rest.client.ExecuteAsync<Dictionary<Guid, Guid>>(restRequest);
        }
        
        /// <summary>
        /// <para>POST: /batch/network/permission</para>
        /// Get Network Permissions by UUIDs
        /// <list type="bullet">
        /// <item><description>Returns what permissions the authenticated user has on the given list of network UUIDs.</description></item>
        /// <item><description>The response payload is a dictionary in which the keys are network UUIDs and the corresponding values are the
        /// highest permission assigned to the authenticated user.</description></item>
        /// <item><description>No more than 500 UUIDs are returned.</description></item>
        /// </list>
        /// <para>Authentication: Required</para>
        /// </summary>
        public async Task<Dictionary<Guid, Permissions>> GetPersonalNetworkPermissions(HashSet<Guid> networkIds)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/batch/network/permission");
            restRequest.SetContentBody(networkIds);
            return await rest.client.ExecuteAsync<Dictionary<Guid, Permissions>>(restRequest);
        }
        
        /// <summary>
        /// <para>POST: /batch/network/summary</para>
        /// Get Network Summaries of UUIDs
        /// <list type="bullet">
        /// <item><description>Returns an array of Network Summary objects.</description></item>
        /// <item><description>The HTTP request payload contains an array of UUIDs which identify which Network Summary objects
        /// to fetch.</description></item>
        /// <item><description>This function only returns summaries for public networks if the user is not authenticated, otherwise it returns
        /// public and the networks for which the authenticated user has READ permission.</description></item>
        /// <item><description>The optional accesskey parameter is supposed to be the accesskey of the network set.
        /// If the given UUID in the POSTed list is a member of that network set,
        /// the server will bypass its permission checking on that network and include the network in the result.</description></item>
        /// <item><description>No more than 2000 UUIDs are returned.</description></item>
        /// </list>
        /// <para>Authentication: Optional</para>
        /// </summary>
        public async Task<List<NetworkSummary>> GetNetworkSummaries(HashSet<Guid> networkIds, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/batch/network/summary");
            if (!string.IsNullOrEmpty(accessKey))
            {
                restRequest.AddUrlSegment("accesskey", accessKey);                
            }

            restRequest.SetContentBody(networkIds);
            return await rest.client.ExecuteAsync<List<NetworkSummary>>(restRequest);
        }

        /// <summary>
        /// <para>POST: /batch/user</para>
        /// Get Users by UUIDs
        /// <list type="bullet">
        /// <item><description>Returns an array of User objects.</description></item>
        /// <item><description>The HTTP request payload contains an array of UUIDs which identify which User objects to fetch.</description></item>
        /// <item><description>No more than 2000 UUIDs are returned.</description></item>
        /// </list>
        /// <para>Authentication: Not required</para>
        /// </summary>
        public async Task<List<model.User>> GetUsers(HashSet<Guid> userIds)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/batch/user");
            restRequest.SetContentBody(userIds);
            return await rest.client.ExecuteAsync<List<model.User>>(restRequest);
        }
    }
}