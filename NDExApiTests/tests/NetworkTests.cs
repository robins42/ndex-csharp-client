using System;
using System.Collections.Generic;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;

namespace NDExApiTests.tests
{
    public class NetworkTests
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetCompleteNetworkAsCxWithError(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUser1NDEx(restImpl)
                    .Network()
                    .GetComplete(Guid.Empty));

            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Network " + Guid.Empty + " not found in NDEx.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetCompleteNetworkAsCx(RestImplementation restImpl)
        {
            string cxJson = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetComplete(SharedIds.NetworkId1);

            Assert.NotNull(cxJson);
            Assert.NotEmpty(cxJson);
            Assert.StartsWith("[{\"numberVerification\":[{\"longNumber\":281474976710655}]}", cxJson);
        }
        
        /// No error case possible, server accepts every string as long as you are authorized
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void CreateNetwork(RestImplementation restImpl)
        {
            Guid networkId = await TemporaryNetworkHelper.CreateNetwork(restImpl);
            TemporaryNetworkHelper.DeleteNetwork(networkId, restImpl);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void UpdateNetwork(RestImplementation restImpl)
        {
            string otherCx = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetComplete(SharedIds.NetworkId2);
            
            Guid networkId = await TemporaryNetworkHelper.CreateNetwork(restImpl);
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .Update(networkId, otherCx);
            
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            TemporaryNetworkHelper.DeleteNetwork(networkId, restImpl);
        }
                
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void TestAccessKeys(RestImplementation restImpl)
        {
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .DisableEnableAccessKey(SharedIds.NetworkId1, AccessKeyAction.disable);
            
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            string accessKey = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetAccessKey(SharedIds.NetworkId1);
            
            Assert.Null(accessKey);
            
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .DisableEnableAccessKey(SharedIds.NetworkId1, AccessKeyAction.enable);
            
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            accessKey = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetAccessKey(SharedIds.NetworkId1);
            
            Assert.NotNull(accessKey);
            Assert.NotEmpty(accessKey);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void MetaDataCollection(RestImplementation restImpl)
        {
            MetadataCollection metadataCollection = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetCxMetadataCollection(SharedIds.NetworkId1);
            
            Assert.NotNull(metadataCollection);
            Assert.NotNull(metadataCollection.metadata);
            Assert.NotEmpty(metadataCollection.metadata);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetAndUpdateNetworkAspect(RestImplementation restImpl)
        {
            Guid networkId = await TemporaryNetworkHelper.CreateNetwork(restImpl);
            List<Dictionary<string, object>> aspectElements = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetAspectElements(networkId, "nodes");
            
            Assert.NotNull(aspectElements);
            Assert.NotEmpty(aspectElements);
            Assert.Equal(12, aspectElements.Count);
            Assert.NotNull(aspectElements[0]["@id"]);

            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .UpdateAspects(networkId, TemporaryNetworkHelper.GetUpdatedCxNetwork());

            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            aspectElements = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetAspectElements(networkId, "nodes");
            
            Assert.NotNull(aspectElements);
            Assert.NotEmpty(aspectElements);
            Assert.Equal(11, aspectElements.Count);
            
            TemporaryNetworkHelper.DeleteNetwork(networkId, restImpl);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetAspectMetadata(RestImplementation restImpl)
        {
            MetadataElement metadata = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetAspectMetadata(SharedIds.NetworkId1, "nodes");
            
            Assert.NotNull(metadata);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void CloneNetwork(RestImplementation restImpl)
        {
            Guid newId = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .Clone(SharedIds.NetworkId1);
            
            Assert.True(Guid.Empty != newId);
            
            TemporaryNetworkHelper.DeleteNetwork(newId, restImpl);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetSetSummaryAndUpdateProfile(RestImplementation restImpl)
        {
            const string oldName = "Cell-Cycle";
            
            // Get old summary
            NetworkSummary summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId1);
            
            Assert.Equal(oldName, summary.name);
            
            // Update it to test value
            summary.name = "test";
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .UpdateProfile(SharedIds.NetworkId1, summary);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check for test value
            summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId1);
            Assert.Equal("test", summary.name);
            
            // Revert it to old name via other method
            summary.name = oldName;
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .UpdateSummary(SharedIds.NetworkId1, summary);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check for old value again
            summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId1);
            Assert.Equal(oldName, summary.name);
        }


        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SetProperties(RestImplementation restImpl)
        {
            // Get old summary
            NetworkSummary summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId2);
            Assert.NotNull(summary.properties);
            
            // Update test value
            foreach (NDExPropertyValuePair property in summary.properties)
            {
                if (property.predicateString != "author") continue;
                property.value = "Test2";
            }
            
            // Send new update
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetProperties(SharedIds.NetworkId2, summary.properties);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check new summary and revert to old value
            summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId2);
            Assert.NotNull(summary.properties);
            
            bool foundProperty = false;
            foreach (NDExPropertyValuePair property in summary.properties)
            {
                if (property.predicateString != "author") continue;
                Assert.Equal("Test2", property.value);
                property.value = "Test";
                foundProperty = true;
            }

            Assert.True(foundProperty);
            
            
            // Reset properties
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetProperties(SharedIds.NetworkId2, summary.properties);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            
            // Check resetted summary
            summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId2);
            Assert.NotNull(summary.properties);
            
            foundProperty = false;
            foreach (NDExPropertyValuePair property in summary.properties)
            {
                if (property.predicateString != "author") continue;
                Assert.Equal("Test", property.value);
                foundProperty = true;
            }
            
            Assert.True(foundProperty);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetAndSetProvenance(RestImplementation restImpl)
        {
            // Check old provenance
            ProvenanceEntity provenance = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetProvenance(SharedIds.NetworkId1);
            Assert.NotNull(provenance);
            Assert.Null(provenance.uri);

            // Update to test value
            provenance.uri = "Test";
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetProvenance(SharedIds.NetworkId1, provenance);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);

            // Check new value
            provenance = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetProvenance(SharedIds.NetworkId1);
            Assert.NotNull(provenance);
            Assert.Equal("Test", provenance.uri);
            
            // Reset to old value
            provenance.uri = null;
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetProvenance(SharedIds.NetworkId1, provenance);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check new old value
            provenance = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetProvenance(SharedIds.NetworkId1);
            Assert.NotNull(provenance);
            Assert.Null(provenance.uri);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void GetAndSetSample(RestImplementation restImpl)
        {
            // Without checking, set to test value 1
            // (with a fresh network the sample can be null, but setting it back to null only sets it to an empty string)
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetSample(SharedIds.NetworkId1, null);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check old sample
            string cx = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSample(SharedIds.NetworkId1);
            Assert.Equal("", cx);

            // Update to test value
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetSample(SharedIds.NetworkId1, "Test");
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);

            // Check new value
            cx = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSample(SharedIds.NetworkId1);
            Assert.Equal("Test", cx);
            
            // Reset to old value
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetSample(SharedIds.NetworkId1, null);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check new old value, again
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetSample(SharedIds.NetworkId1, null);
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SetFlag(RestImplementation restImpl)
        {
            // Set to private
            RestResponse response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetFlag(SharedIds.NetworkId1, new NetworkSystemProperties
                {
                    visibility = Visibility.PRIVATE
                });
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check private value
            NetworkSummary summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId1);
            Assert.NotNull(summary);
            Assert.Equal(Visibility.PRIVATE, summary.visibility);

            // Set to public
            response = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .SetFlag(SharedIds.NetworkId1, new NetworkSystemProperties
                {
                    visibility = Visibility.PUBLIC
                });
            Assert.NotNull(response);
            Assert.True(response.wasSuccess);
            
            // Check public value
            summary = await Utils.GetUser1NDEx(restImpl)
                .Network()
                .GetSummary(SharedIds.NetworkId1);
            Assert.NotNull(summary);
            Assert.Equal(Visibility.PUBLIC, summary.visibility);
        }
    }
}