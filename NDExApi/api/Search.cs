using System;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApi.api
{
    /// <summary>
    /// baseUrl/search
    /// </summary>
    public class Search
    {
        private readonly NetworkConfiguration _network;
        
        internal Search(NetworkConfiguration network)
        {
            _network = network;
        }
        
        /// <summary>
        /// <para>POST: /search/group</para>
        /// </summary>
        public async Task<SearchResult<model.Group>> FindGroups(string queryString, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/search/group");
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount);
            restRequest.SetContentBody(new SimpleQuery {searchString = queryString});
            return await _network.client.ExecuteAsync<SearchResult<model.Group>>(restRequest);
        }
        
        /// <summary>
        /// <para>POST: /search/network</para>
        /// </summary>
        public async Task<SearchResult<NetworkSummary>> SearchNetwork(string queryString, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/search/network");
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount);
            restRequest.SetContentBody(new SimpleQuery {searchString = queryString});
            NetworkSearchResult result = await _network.client.ExecuteAsync<NetworkSearchResult>(restRequest);
            return new SearchResult<NetworkSummary>
            {
                start = result.start,
                numFound = result.numFound,
                resultList = result.networks
            };
        }
        
        /// <summary>
        /// <para>POST: /search/network/genes</para>
        /// </summary>
        public async Task<SearchResult<NetworkSummary>> SearchNetworkByGenes(string queryString, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/search/network/genes");
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount);
            restRequest.SetContentBody(new SimpleQuery {searchString = queryString});
            NetworkSearchResult result = await _network.client.ExecuteAsync<NetworkSearchResult>(restRequest);
            return new SearchResult<NetworkSummary>
            {
                start = result.start,
                numFound = result.numFound,
                resultList = result.networks
            };
        }
        
//        /// <summary>
//        /// <para>POST: /search/network/{networkid}/advancedquery</para>
//        /// </summary>
//        public async Task<string> AdvancedQuery(Guid networkId, EdgeCollectionQuery queryParameters, string accessKey = null)
//        {
//            RestRequest restRequest = new RestRequest(RestMethod.POST, "/search/network/" + networkId + "/advancedquery");
//            restRequest.AddUrlSegment("accesskey", accessKey);
//            restRequest.SetContentBody(queryParameters);
//            RestResponse response = await _network.RestClient.ExecuteAsync(restRequest);
//            return response.json;
//        }
                
        /// <summary>
        /// <para>POST: /search/network/{networkid}/interconnectquery</para>
        /// </summary>
        public async Task<string> InterconnectQuery(
            Guid networkId, SimplePathQuery queryParameters, bool saveAsNetwork = false, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/search/network/" + networkId + "/interconnectquery");
            restRequest.AddUrlSegment("accesskey", accessKey);
            restRequest.AddUrlSegment("save", saveAsNetwork);
            restRequest.SetContentBody(queryParameters);
            RestResponse response = await _network.client.ExecuteAsync(restRequest);
            return response.json;
        }
        
        /// <summary>
        /// <para>POST: /search/network/{networkid}/query</para>
        /// </summary>
        public async Task<string> QueryNetworkAsCx(
            Guid networkId, SimplePathQuery queryParameters, bool saveAsNetwork = false, string accessKey = null)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/search/network/" + networkId + "/query");
            restRequest.AddUrlSegment("accesskey", accessKey);
            restRequest.AddUrlSegment("save", saveAsNetwork);
            restRequest.SetContentBody(queryParameters);
            RestResponse response = await _network.client.ExecuteAsync(restRequest);
            return response.json;
        }
                
        /// <summary>
        /// <para>POST: /search/user</para>
        /// </summary>
        public async Task<SearchResult<model.User>> FindUsers(string queryString, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.POST, "/search/user");
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount);
            restRequest.SetContentBody(new SimpleQuery {searchString = queryString});
            return await _network.client.ExecuteAsync<SearchResult<model.User>>(restRequest);
        }
    }
}