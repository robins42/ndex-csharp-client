using System;
using System.Collections.Generic;
using System.Threading;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;

namespace NDExApiTests.tests
{
    public class NetworkSetTests
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void Crud(RestImplementation restImpl)
        {
            Guid id = await TemporaryNetworkSetHelper.CreateNetworkSet(restImpl);
            
            // Read set
            NetworkSet liveSet = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Get(id);
            Assert.NotNull(liveSet);
            Assert.Equal(SharedIds.UserId1, liveSet.ownerId);
            Assert.NotNull(liveSet.networks);
            Assert.Empty(liveSet.networks);
            Assert.StartsWith("Test Set", liveSet.name);
            
            // Update set
            liveSet.name = "Test Set 2";
            liveSet.networks.Add(SharedIds.NetworkId1);
            RestResponse response = await Utils.GetUser1NDEx(restImpl).NetworkSet()
                .Update(id, liveSet);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check update
            liveSet = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Get(id);
            Assert.NotNull(liveSet);
            Assert.NotNull(liveSet.networks);
            Assert.Empty(liveSet.networks);
            Assert.Equal("Test Set 2", liveSet.name);
            
            // Delete set
            TemporaryNetworkSetHelper.DeleteNetworkSet(id, restImpl);
            
            // Sometimes the set is not deleted when the check is reached, so add a buffer
            Thread.Sleep(1000);
            
            // Check deletion
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .NetworkSet()
                    .Get(id));
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Network set" + id + " not found in db.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void ManageNetworks(RestImplementation restImpl)
        {
            Guid id = await TemporaryNetworkSetHelper.CreateNetworkSet(restImpl);
            
            // Add networks
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .AddNetworks(id, new HashSet<Guid>
                {
                    SharedIds.NetworkId1,
                    SharedIds.NetworkId2
                });
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check both networks
            NetworkSet liveSet = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Get(id);
            Assert.NotNull(liveSet);
            Assert.Contains(SharedIds.NetworkId1, liveSet.networks);
            Assert.Contains(SharedIds.NetworkId2, liveSet.networks);
            
            // Delete 1 network
            response = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .RemoveNetworks(id, new HashSet<Guid>
                {
                    SharedIds.NetworkId2
                });
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check deletion
            liveSet = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Get(id);
            Assert.NotNull(liveSet);
            Assert.Contains(SharedIds.NetworkId1, liveSet.networks);
            Assert.DoesNotContain(SharedIds.NetworkId2, liveSet.networks);
            
            TemporaryNetworkSetHelper.DeleteNetworkSet(id, restImpl);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SetFlag(RestImplementation restImpl)
        {
            Guid id = await TemporaryNetworkSetHelper.CreateNetworkSet(restImpl);
            
            // Set to new value
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .SetFlag(id, new NetworkSetSystemProperties
                {
                    showcase = true
                });
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check new value
            NetworkSet set = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Get(id);
            Assert.NotNull(set);
            Assert.True(set.showcased);

            // Reset to old value
            response = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .SetFlag(id, new NetworkSetSystemProperties
                {
                    showcase = false
                });
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check old value
            set = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Get(id);
            Assert.NotNull(set);
            Assert.False(set.showcased);
            
            TemporaryNetworkSetHelper.DeleteNetworkSet(id, restImpl);
        }
    }
}