using System;
using System.Collections.Generic;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;

namespace NDExApiTests.tests
{
    public class BatchTests
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void ExportWithError(RestImplementation restImpl)
        {
            NetworkExportRequest exportRequest = new NetworkExportRequest
            {
                exportFormat = null,
                networkIds = new HashSet<Guid>
                {
                    SharedIds.NetworkId1
                }
            };
            
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Batch()
                    .ExportNetworks(exportRequest));
            
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("No exporter was registered in this server for network format null", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void ExportGraphMlFormatMultiple(RestImplementation restImpl)
        {
            NetworkExportRequest exportRequest = new NetworkExportRequest
            {
                exportFormat = NetworkExportRequest.ExportFormatGraphML,
                networkIds = new HashSet<Guid>
                {
                    SharedIds.NetworkId1,
                    SharedIds.NetworkId2
                }
            };

            Dictionary<Guid, Guid> taskAssignment = await Utils.GetUser1NDEx(restImpl)
                .Batch()
                .ExportNetworks(exportRequest);
            
            Assert.NotNull(taskAssignment);
            Assert.NotEmpty(taskAssignment);
            Assert.Equal(2, taskAssignment.Count);
            Assert.True(taskAssignment.ContainsKey(SharedIds.NetworkId1));
            Assert.True(taskAssignment.ContainsKey(SharedIds.NetworkId2));
            
            RestResponse deletion1 = await Utils.GetUser1NDEx(restImpl)
                .Task()
                .DeleteTask(taskAssignment[SharedIds.NetworkId1]);
            Assert.True(deletion1.wasSuccess);
            
            RestResponse deletion2 = await Utils.GetUser1NDEx(restImpl)
                .Task()
                .DeleteTask(taskAssignment[SharedIds.NetworkId2]);
            Assert.True(deletion2.wasSuccess);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetGroupsWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Batch()
                    .GetGroupsByIds(null));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Request body cannot be null.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetGroupsByIdsMultipleList(RestImplementation restImpl)
        {
            List<Group> group = await Utils.GetUser1NDEx(restImpl)
                .Batch()
                .GetGroupsByIds(new HashSet<Guid>
                {
                    SharedIds.GroupId1,
                    SharedIds.GroupId2
                });

            Assert.NotNull(group);
            Assert.Equal(2, group.Count);
            Assert.Equal(SharedIds.GroupId1, group[0].externalId);
            Assert.Equal(SharedIds.GroupId2, group[1].externalId);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void PermissionWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Batch()
                    .GetPersonalNetworkPermissions(null));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Request body cannot be null.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void PermissionMultipleSet(RestImplementation restImpl)
        {
            HashSet<Guid> networkIds = new HashSet<Guid>
            {
                SharedIds.NetworkId1,
                SharedIds.NetworkId2
            };

            Dictionary<Guid, Permissions> permissions = await Utils.GetUser1NDEx(restImpl)
                .Batch()
                .GetPersonalNetworkPermissions(networkIds);

            Assert.NotNull(permissions);
            Assert.NotEmpty(permissions);
            Assert.Equal(2, permissions.Count);
            Assert.True(permissions.ContainsKey(SharedIds.NetworkId1));
            Assert.True(permissions.ContainsKey(SharedIds.NetworkId2));
            Assert.Equal(Permissions.ADMIN, permissions[SharedIds.NetworkId1]);
            Assert.Equal(Permissions.ADMIN, permissions[SharedIds.NetworkId2]);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SummaryWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Batch()
                    .GetNetworkSummaries(null));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Request body cannot be null.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SummaryWithMissingAccessKey(RestImplementation restImpl)
        {
            HashSet<Guid> networkIds = new HashSet<Guid>
            {
                SharedIds.NetworkId1,
                SharedIds.NetworkId2,
                SharedIds.NetworkId3
            };

            List<NetworkSummary> summaries = await Utils.GetUser1NDEx(restImpl)
                .Batch()
                .GetNetworkSummaries(networkIds);

            Assert.NotNull(summaries);
            Assert.NotEmpty(summaries);
            Assert.Equal(2, summaries.Count);
            bool foundNetwork1 = summaries[0].externalId == SharedIds.NetworkId1 ||
                                 summaries[1].externalId == SharedIds.NetworkId1;
            Assert.True(foundNetwork1);
            bool foundNetwork2 = summaries[0].externalId == SharedIds.NetworkId2 ||
                                 summaries[1].externalId == SharedIds.NetworkId2;
            Assert.True(foundNetwork2);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SummaryWithAccessKey(RestImplementation restImpl)
        {
            HashSet<Guid> networkIds = new HashSet<Guid>
            {
                SharedIds.NetworkId1,
                SharedIds.NetworkId2,
                SharedIds.NetworkId3
            };

            List<NetworkSummary> summaries = await Utils.GetUser1NDEx(restImpl)
                .Batch()
                .GetNetworkSummaries(networkIds, SharedIds.Network3AccessKey);

            Assert.NotNull(summaries);
            Assert.NotEmpty(summaries);
            Assert.Equal(3, summaries.Count);
            foreach (NetworkSummary summary in summaries)
            {
                Assert.True(SharedIdsContain(summary.externalId));
            }
        }

        private static bool SharedIdsContain(Guid id)
        {
            return id == SharedIds.NetworkId1 || id == SharedIds.NetworkId2 || id == SharedIds.NetworkId3;
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetUsersWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Batch()
                    .GetUsers(null));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Request body cannot be null.", exception.Message);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetUsersMultiple(RestImplementation restImpl)
        {
            List<User> users = await Utils.GetUser1NDEx(restImpl)
                .Batch()
                .GetUsers(new HashSet<Guid>
                {
                    SharedIds.UserId1,
                    SharedIds.UserId2
                });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal(SharedIds.UserId1, users[0].externalId);
            Assert.Equal(SharedIds.UserId2, users[1].externalId);
        }
    }
}