using System;
using System.Collections.Generic;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;
using NetworkSet = NDExApi.model.NetworkSet;

namespace NDExApiTests.tests
{
    public class UserTests
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void PermissionSpecificGroup(RestImplementation restImpl)
        {
            Permissions permissions = await Utils.GetUser1NDEx(restImpl)
                .User()
                .GetMembershipOfSpecificGroup(SharedIds.UserId1, SharedIds.GroupId1);
            Assert.Equal(Permissions.GROUPADMIN, permissions);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void PermissionAllGroups(RestImplementation restImpl)
        {
            Dictionary<Guid, Permissions> permissions = await Utils.GetUser1NDEx(restImpl)
                .User()
                .GetMembershipOfAllGroups(SharedIds.UserId1, Permissions.GROUPADMIN);
            Assert.NotNull(permissions);
            Assert.NotEmpty(permissions);
            Assert.True(permissions.ContainsKey(SharedIds.GroupId1));
            Assert.True(permissions.ContainsKey(SharedIds.GroupId2));
            Assert.Equal(Permissions.GROUPADMIN, permissions[SharedIds.GroupId1]);
            Assert.Equal(Permissions.GROUPADMIN, permissions[SharedIds.GroupId2]);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void NetworkSets(RestImplementation restImpl)
        {
            List<NetworkSet> permissions = await Utils.GetUser1NDEx(restImpl)
                .User()
                .GetNetworkSets(SharedIds.UserId1);
            Assert.NotNull(permissions);
            Assert.Empty(permissions);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void NetworkSummaries(RestImplementation restImpl)
        {
            List<NetworkSummary> summaries = await Utils.GetUser1NDEx(restImpl)
                .User()
                .GetNetworkSummaries(SharedIds.UserId1);
            Assert.NotNull(summaries);

            bool network1Found = false;
            bool network2Found = false;
            foreach (NetworkSummary summary in summaries)
            {
                if (summary.externalId == SharedIds.NetworkId1) network1Found = true;
                else if (summary.externalId == SharedIds.NetworkId2) network2Found = true;
            }
            
            Assert.True(network1Found);
            Assert.True(network2Found);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void NetworkPermissionInfo(RestImplementation restImpl)
        {
            Permissions permission = await Utils.GetUser1NDEx(restImpl)
                .User()
                .GetNetworkPermissionInfo(SharedIds.UserId1, SharedIds.NetworkId1);
            Assert.Equal(Permissions.ADMIN, permission);
        }
    }
}