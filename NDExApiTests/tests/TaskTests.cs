using System;
using System.Collections.Generic;
using System.Threading;
using NDExApi.api;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;
using Task = NDExApi.model.Task;

namespace NDExApiTests.tests
{
    public class TaskTests
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void TestAll(RestImplementation restImpl)
        {
            // ID from a network on the live server this account owns
            Guid networkId = new Guid("d30013ce-83a9-11e9-848d-0ac135e8bacf");
            NDEx ndex = Utils.GetUser1NDExLive(restImpl);
         
            // Create a task
            Dictionary<Guid, Guid> exportTasks = await ndex
                .Batch()
                .ExportNetworks(new NetworkExportRequest
                {
                    exportFormat = NetworkExportRequest.ExportFormatGraphML,
                    networkIds = new HashSet<Guid>
                    {
                        networkId
                    }
                });

            Guid taskId = exportTasks[networkId];
            
            // Give server some time to complete
            Thread.Sleep(5000);
            
            // Download from task
            string downloadedFile = await ndex
                .Task()
                .DownloadExportedFile(taskId);
            Assert.NotNull(downloadedFile);

            // Get specific task
            Task specificTask = await ndex.Task().GetSpecificTask(taskId);
            Assert.NotNull(specificTask);

            // Get all tasks
            List<Task> tasks = await ndex
                .Task()
                .GetAllTasks();
            Assert.NotNull(tasks);
            Assert.NotEmpty(tasks);
            
            // Delete all tasks
            foreach (Task t in tasks)
            {
                RestResponse response = await ndex.Task().DeleteTask(t.externalId);
                Assert.NotNull(response);
                Assert.True(response.wasSuccess);
            }
        }
    }
}