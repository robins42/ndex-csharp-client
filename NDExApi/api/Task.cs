using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDExApi.rest;
using TaskStatus = NDExApi.model.TaskStatus;

namespace NDExApi.api
{
    /// <summary>
    /// baseUrl/task
    /// </summary>
    public class Task
    {
        private readonly NetworkConfiguration _network;
        
        internal Task(NetworkConfiguration network)
        {
            _network = network;
        }
        
        /// <summary>
        /// <para>GET: /task</para>
        /// </summary>
        public async Task<List<model.Task>> GetAllTasks(TaskStatus status = TaskStatus.ALL, int startIndex = 0, int amount = 100)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/task");
            restRequest.AddUrlSegment("status", status);
            restRequest.AddUrlSegment("start", startIndex);
            restRequest.AddUrlSegment("size", amount); 
            return await _network.client.ExecuteAsync<List<model.Task>>(restRequest);
        }
        
        /// <summary>
        /// <para>GET: /task/{taskid}</para>
        /// </summary>
        public async Task<model.Task> GetSpecificTask(Guid taskId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/task/" + taskId);
            return await _network.client.ExecuteAsync<model.Task>(restRequest);
        }
        
        /// <summary>
        /// <para>DELETE: /task/{taskid}</para>
        /// </summary>
        public async Task<RestResponse> DeleteTask(Guid taskId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.DELETE, "/task/" + taskId);
            return await _network.client.ExecuteAsync(restRequest);
        }
                
        /// <summary>
        /// <para>GET: /task/{taskid}/file</para>
        /// </summary>
        public async Task<string> DownloadExportedFile(Guid taskId)
        {
            RestRequest restRequest = new RestRequest(RestMethod.GET, "/task/" + taskId + "/file");
            RestResponse response = await _network.client.ExecuteAsync(restRequest);
            return response.json;
        }
    }
}

