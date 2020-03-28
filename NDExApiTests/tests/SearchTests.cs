using System.Collections.Generic;
using NDExApi.api;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;
using Group = NDExApi.model.Group;
using User = NDExApi.model.User;

namespace NDExApiTests.tests
{
    public class SearchTests
    {
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void FindUsers(RestImplementation restImpl)
        {
            SearchResult<User> searchResult = await Utils.GetUser1NDEx(restImpl)
                .Search()
                .FindUsers("robin.schoenborn");

            Assert.NotNull(searchResult);
            Assert.Equal(0, searchResult.start);
            Assert.True(searchResult.numFound > 0);
            Assert.NotNull(searchResult.resultList);
            Assert.NotEmpty(searchResult.resultList);
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void FindGroups(RestImplementation restImpl)
        {
            SearchResult<Group> searchResult = await Utils.GetUser1NDEx(restImpl)
                .Search()
                .FindGroups("Test Gruppe");

            Assert.NotNull(searchResult);
            Assert.Equal(0, searchResult.start);
            Assert.True(searchResult.numFound > 0);
            Assert.NotNull(searchResult.resultList);
            Assert.NotEmpty(searchResult.resultList);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SearchNetwork(RestImplementation restImpl)
        {
            SearchResult<NetworkSummary> searchResult = await Utils.GetUser1NDEx(restImpl)
                .Search()
                .SearchNetwork("\"Copy of ID Signalling Pathway\"");

            Assert.NotNull(searchResult);
            Assert.Equal(0, searchResult.start);
            Assert.True(searchResult.numFound > 0);
            Assert.NotNull(searchResult.resultList);
            Assert.NotEmpty(searchResult.resultList);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void SearchNetworkByGenes(RestImplementation restImpl)
        {
            SearchResult<NetworkSummary> searchResult = await Utils.GetUser1NDEx(restImpl)
                .Search()
                .SearchNetworkByGenes("AKT1");

            Assert.NotNull(searchResult);
            Assert.Equal(0, searchResult.start);
            Assert.True(searchResult.numFound > 0);
            Assert.NotNull(searchResult.resultList);
            Assert.NotEmpty(searchResult.resultList);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void QueryNetworkAsCx(RestImplementation restImpl)
        {
            string cxJson = await Utils.GetUser1NDEx(restImpl)
                .Search()
                .QueryNetworkAsCx(SharedIds.NetworkId1, new SimplePathQuery
                {
                    searchString = "\"Copy of ID Signalling Pathway\""
                });

            Assert.NotNull(cxJson);
            Assert.NotEmpty(cxJson);
            Assert.StartsWith("[{\"numberVerification\":[{\"longNumber\":281474976710655}]}", cxJson);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void InterconnectQuery(RestImplementation restImpl)
        {
            string cxJson = await Utils.GetUser1NDEx(restImpl)
                .Search()
                .InterconnectQuery(SharedIds.NetworkId1,
                    new SimplePathQuery
                    {
                        searchString = "Copy of ESAD-2017-WNT-CTNNB1-pathway"
                    });

            Assert.NotNull(cxJson);
            Assert.NotEmpty(cxJson);
            Assert.StartsWith("[{\"numberVerification\":[{\"longNumber\":281474976710655}]}", cxJson);
        }
        
//        [Theory]
//        [ClassData(typeof(RestClientTheories))]
//        public async void AdvancedQuery()
//        {
//            EdgeCollectionQuery query = new EdgeCollectionQuery
//            {
//                queryName = "foo",
//                edgeLimit = 100,
//                edgeFilter = new EdgeByEdgePropertyFilter
//                {
//                    propertySpecifications = new List<PropertySpecification>
//                    {
//                        new PropertySpecification
//                        {
//                            name = "ndex:predicate",
//                            value = "DIRECTLY_INCREASES"
//                        }
//                    }
//                },
//                nodeFilter = new EdgeByNodePropertyFilter
//                {
//                    mode = SpecMatchMode.Source,
//                    propertySpecifications = new List<PropertySpecification>
//                    {
//                        new PropertySpecification
//                        {
//                            name = "ndex:nodeName",
//                            value = "test"
//                        }
//                    }
//                }
//            };
//            string cxJson = await Utils.GetUser1NDEx(restImpl)
//                .Search()
//                .AdvancedQuery(SharedIds.NetworkId1, query);
//
//            Assert.NotNull(cxJson);
//            Assert.NotEmpty(cxJson);
//            Assert.StartsWith("[{\"numberVerification\":[{\"longNumber\":281474976710655}]}", cxJson);
//        }
    }
}