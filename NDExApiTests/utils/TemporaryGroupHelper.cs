using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NDExApi.model;
using NDExApi.rest;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace NDExApiTests.utils
{
    public static class TemporaryGroupHelper
    {
        private const string JsonStart = "http://dev2.ndexbio.org/v2/group/";
        
        public static Group GetTestGroup()
        {
            return new Group
            {
                groupName = "Dynamic Group - " + Guid.NewGuid(),
                image = "https://i.postimg.cc/5yKjqqMH/test-icon-3.png",
                description = "This group was created by a test, should be destroyed later",
                website = "http://test.ndexbio.org",
                properties = new Dictionary<string, object>() //Properties for this seem to be dropped by NDEx
            };
        }
        
        public static Group GetTestGroupUpdated()
        {
            return new Group
            {
                groupName = "Dynamic Group - " + Guid.NewGuid(),
                image = "https://i.postimg.cc/7YY5SVz1/test-icon-4.png",
                description = "This group was created by a test, should be destroyed later!",
                website = "http://test.ndexbio.org/",
                properties = new Dictionary<string, object>() //Properties for this seem to be dropped by NDEx
            };
        }

        public static async Task<Group> CreateTestGroup(RestImplementation restImpl)
        {
            RestResponse creationResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Create(GetTestGroup());

            Assert.NotNull(creationResponse);
            Assert.True(creationResponse.wasSuccess);
            Assert.StartsWith(JsonStart, creationResponse.json);

            Guid id = new Guid(creationResponse.json.Substring(JsonStart.Length));
            return await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Get(id);
        }

        public static async Task DeleteTestGroup(Guid createdGroupId, RestImplementation restImpl)
        {
            RestResponse deleteResponse = await Utils.GetUser1NDEx(restImpl)
                .Group()
                .Delete(createdGroupId);
            Assert.True(deleteResponse.wasSuccess);
        }
    }
}