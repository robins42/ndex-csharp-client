using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;

namespace NDExApiTests.tests
{
    public class AdminTest
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetStatusShort(RestImplementation restImpl)
        {
            NDExStatus status = await Utils.GetUser1NDEx(restImpl)
                .Admin()
                .GetStatus(AdminStatusFormat.Short);
            
            AssertCommonAttributesOfStatus(status);
            Assert.False(status.properties.ContainsKey("ImporterExporters"));
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetStatusFull(RestImplementation restImpl)
        {
            NDExStatus status = await Utils.GetUser1NDEx(restImpl)
                .Admin()
                .GetStatus();
            
            AssertCommonAttributesOfStatus(status);
            Assert.True(status.properties.ContainsKey("ImporterExporters"));
        }

        private static void AssertCommonAttributesOfStatus(NDExStatus status)
        {
            Assert.NotNull(status);
            Assert.Equal("Online", status.message);
            Assert.NotNull(status.properties);
            Assert.True(status.properties.ContainsKey("ServerVersion"));
            Assert.StartsWith("2.4", status.properties["ServerVersion"].ToString());
        }
    }
}