using System;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;
using Xunit;

namespace NDExApiTests.utils
{
    public static class TemporaryNetworkSetHelper
    {
        public static async Task<Guid> CreateNetworkSet(RestImplementation restImpl)
        {
            NetworkSet craftedSet = new NetworkSet
            {
                ownerId = SharedIds.UserId1,
                name = "Test Set - " + new Random().Next()
            };
            
            // Create set
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Create(craftedSet);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // network id from response
            return Utils.GetIdFromUrl(response.json);
        }

        public static async void DeleteNetworkSet(Guid networkId, RestImplementation restImpl)
        {
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .NetworkSet()
                .Delete(networkId);
            
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
        }
    }
}