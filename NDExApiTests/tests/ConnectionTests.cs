using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NDExApi.api;
using NDExApi.model;
using NDExApi.rest;
using NDExApiTests.utils;
using Xunit;
using Xunit.Abstractions;

namespace NDExApiTests.tests
{
    public class ConnectionTests
    {
        // These attributes had been set to a specific proxy with some errors, this was changed to dummy data for GitHub for data security protection
        private static readonly NDExProxy Proxy = new NDExProxy("proxy", 1234);
        private static readonly NDExProxy BadProxyName = new NDExProxy("proxz", 1234);
        private static readonly NDExProxy BadProxyPort = new NDExProxy("proxy", 1235);
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void WrongBaseUrl(RestImplementation restImpl)
        {
            NDExProxy optionalProxy = Utils.UseProxy ? Proxy : null;
            NetworkConfiguration conf = new NetworkConfiguration
            {
                ndexUrlBase = "http://xtestx.xndexbio.xorg/xv2",
                proxy = optionalProxy,
                implementation = restImpl
            };
            Exception ex = await Assert.ThrowsAnyAsync<Exception>(() =>
                new NDEx(conf)
                    .Network()
                    .GetSummary(SharedIds.NetworkId1));
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void MissingAuthentication(RestImplementation restImpl)
        {
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>
                Utils.GetUnauthenticatedNDEx(restImpl)
                    .Group()
                    .Create(TemporaryGroupHelper.GetTestGroup()));
            
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.EndsWith("Attempted to access resource requiring authentication.", exception.Message);
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
//        [InlineData(PreferredRestImplementation.WebClient)]
        public async void WrongUsername(RestImplementation restImpl)
        {
            Authentication auth = new Authentication("<YOUR_WRONG_USERNAME>", "<YOUR_CORRECT_PASSWORD>");
            NetworkConfiguration network = new NetworkConfiguration
            {
                ndexUrlBase = Utils.Url,
                proxy = Utils.UseProxy ? Utils.Proxy : null,
                authentication = auth,
                implementation = restImpl
            };
            
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>                         
                new NDEx(network)                                           
                    .Group()                                                                                
                    .Create(TemporaryGroupHelper.GetTestGroup()));                                     
                                                                                                            
            Assert.NotNull(exception);                                                                      
            Assert.NotNull(exception.Message);                                                              
            Assert.EndsWith("User <YOUR_WRONG_USERNAME> doesn't exist.", exception.Message);      
        }
        
        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void WrongPassword(RestImplementation restImpl)
        {
            Authentication auth = new Authentication("<YOUR_CORRECT_USERNAME>", "<YOUR_WRONG_PASSWORD>");
            
            NetworkConfiguration network = new NetworkConfiguration
            {
                ndexUrlBase = Utils.Url,
                proxy = Utils.UseProxy ? Utils.Proxy : null,
                authentication = auth,
                implementation = restImpl
            };
            
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>                         
                new NDEx(network)                                           
                    .Group()                                                                                
                    .Create(TemporaryGroupHelper.GetTestGroup()));                                     
                                                                                                            
            Assert.NotNull(exception);                                                                      
            Assert.NotNull(exception.Message);                                                              
            Assert.Contains("Invalid password for user", exception.Message);      
        }

        [Theory]
        [ClassData(typeof(RestClientTheories))]
        public async void WrongOAuth(RestImplementation restImpl)
        {
            const string token = "<YOUR_OAUTH_TOKEN>";

            Authentication auth = new Authentication(token);
            NetworkConfiguration network = new NetworkConfiguration
            {
                ndexUrlBase = Utils.Url,
                proxy = Utils.UseProxy ? Utils.Proxy : null,
                authentication = auth,
                implementation = restImpl
            };
            
            NDExException exception = await Assert.ThrowsAsync<NDExException>(() =>                         
                new NDEx(network)                                           
                    .Group()                                                                                
                    .Create(TemporaryGroupHelper.GetTestGroup()));                                     
                                                                                                            
            Assert.NotNull(exception);                                                                      
            Assert.NotNull(exception.Message);                                                              
            Assert.Contains("Invalid password for user", exception.Message);      
        }
        
//        [Fact]
//        public async void PerformanceTest()
//        {
//            const int maxLoops = 100;
//            
//            Erg ergHttpClient = await runPerformanceTest(RestImplementation.HttpClient, maxLoops);
//            Erg ergWebClient = await runPerformanceTest(RestImplementation.WebClient, maxLoops);
//            Erg ergHttpWebRequest = await runPerformanceTest(RestImplementation.HttpWebRequest, maxLoops);
//            
//            long msPerRun1 = ergHttpClient.totalMs / maxLoops;
//            long msPerRun2 = ergWebClient.totalMs / maxLoops;
//            long msPerRun3 = ergHttpWebRequest.totalMs / maxLoops;
//            
//            logger.WriteLine("HTTPClient: Executed WS " + maxLoops + " times in " + ergHttpClient.totalMs + " ms => " + msPerRun1 + " ms per WS, "
//                             + ergHttpClient.minMs + " min, " + ergHttpClient.maxMs + " max");
//            string allT = "";
//            foreach (long time in ergHttpClient.allTimes)
//            {
//                if (allT.Length > 0) allT += ", ";
//                allT += "" + time;
//            }
//            logger.WriteLine("---- " + allT);
//            logger.WriteLine("WebClient: Executed WS " + maxLoops + " times in " + ergWebClient.totalMs + " ms => " + msPerRun2 + " ms per WS, "
//                             + ergWebClient.minMs + " min, " + ergWebClient.maxMs + " max");
//            allT = "";
//            foreach (long time in ergWebClient.allTimes)
//            {
//                if (allT.Length > 0) allT += ", ";
//                allT += "" + time;
//            }
//            logger.WriteLine("---- " + allT);
//            logger.WriteLine("HTTPWebRequest: Executed WS " + maxLoops + " times in " + ergHttpWebRequest.totalMs + " ms => " + msPerRun3 + " ms per WS, "
//                             + ergHttpWebRequest.minMs + " min, " + ergHttpWebRequest.maxMs + " max");
//            allT = "";
//            foreach (long time in ergHttpWebRequest.allTimes)
//            {
//                if (allT.Length > 0) allT += ", ";
//                allT += "" + time;
//            }
//            logger.WriteLine("---- " + allT);
//        }
//        
//        private readonly ITestOutputHelper logger;
//
//        public ConnectionTests(ITestOutputHelper output)
//        {
//            this.logger = output;
//        }
//
//        private class Erg
//        {
//            internal long minMs, maxMs, totalMs;
//            internal List<long> allTimes;
//        }
//
//        private static async Task<Erg> runPerformanceTest(RestImplementation restImpl, int maxLoops)
//        {
//            NDEx ndex = Utils.GetUser1NDEx(restImpl);
//            Stopwatch watch = new Stopwatch();
//            long min = long.MaxValue, max = 0, total = 0, temp;
//            List<long> allTimess = new List<long>(maxLoops);
//            
//            for (int i = 0; i < maxLoops+1; i++)
//            {
//                watch.Start();
//                try
//                {
//                    await ndex
//                        .Task()
//                        .GetAllTasks();
//                }
//                catch (NDExException)
//                {
//                }
//                watch.Stop();
//                if (i > 0)
//                {
//                    temp = watch.ElapsedMilliseconds;
//                    if (temp < min) min = temp;
//                    if (temp > max) max = temp;
//                    total += temp;
//                    allTimess.Add(temp);
//                }
//
//                watch.Reset();
//            }
//
//            return new Erg()
//            {
//                minMs = min,
//                maxMs = max,
//                totalMs = total,
//                allTimes = allTimess
//            };
//        }

//        [Fact]
//        public async void DeleteAllDebugGroups()
//        {
//            Dictionary<Guid,Permissions> permissions = await Utils.GetUser1NDEx(RestImplementation.HttpWebRequest)
//                .User()
//                .GetMembershipOfAllGroups(SharedIds.UserId1, Permissions.GROUPADMIN);
//
//            permissions.Remove(SharedIds.GroupId1);
//            permissions.Remove(SharedIds.GroupId2);
//
//            foreach (Guid id in permissions.Keys)
//            {
//                await Utils.GetUser1NDEx(RestImplementation.HttpWebRequest)
//                    .Group()
//                    .Delete(id);
//            }
//        }
//
//        [Fact]
//        public async void FuerAbbildung()
//        {
//            NDEx ndex = Utils.GetUser1NDEx(RestImplementation.HttpWebRequest);
//            Guid id = Guid.Empty;
//            Guid groupId = Guid.Empty;
//            
//            await ndex.Group().Get(Guid.Empty);
//        }
        
        [TheoryWithProxy]
        [ClassData(typeof(RestClientTheories))]
        public async void WrongProxyUrl(RestImplementation restImpl)
        {
            NetworkConfiguration conf = new NetworkConfiguration
            {
                ndexUrlBase = "http://testx.ndexbio.org/v2",
                proxy = BadProxyName,
                implementation = restImpl
            };
            Exception ex = await Assert.ThrowsAnyAsync<Exception>(() =>
                new NDEx(conf)
                    .Network()
                    .GetSummary(SharedIds.NetworkId1));
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
        }

        [TheoryWithProxy]
        [ClassData(typeof(RestClientTheories))]
        public async void WrongProxyPort(RestImplementation restImpl)
        {
            NetworkConfiguration conf = new NetworkConfiguration
            {
                ndexUrlBase = "http://testx.ndexbio.org/v2",
                proxy = BadProxyPort,
                implementation = restImpl
            };
            Exception ex = await Assert.ThrowsAnyAsync<Exception>(() =>
                new NDEx(conf)
                    .Network()
                    .GetSummary(SharedIds.NetworkId1));
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
        }

        [TheoryWithProxy]
        [ClassData(typeof(RestClientTheories))]        
        public async void MissingProxy(RestImplementation restImpl)
        {
            NetworkConfiguration conf = new NetworkConfiguration
            {
                ndexUrlBase = "http://testx.ndexbio.org/v2",
                implementation = restImpl
            };
            Exception ex = await Assert.ThrowsAnyAsync<Exception>(() =>
                new NDEx(conf)
                    .Network()
                    .GetSummary(SharedIds.NetworkId1));
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
            
            NetworkConfiguration conf2 = new NetworkConfiguration
            {
                ndexUrlBase = "http://testx.ndexbio.org/v2",
                authentication = new Authentication("test", "test"),
                implementation = restImpl
            };
            ex = await Assert.ThrowsAnyAsync<Exception>(() =>
                new NDEx(conf2)
                    .Network()
                    .GetSummary(SharedIds.NetworkId1));
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
        }
        
        [TheoryWithoutProxy]
        [ClassData(typeof(RestClientTheories))]
        public async void UnusedProxy(RestImplementation restImpl)
        {
            NetworkConfiguration conf = new NetworkConfiguration
            {
                ndexUrlBase = "http://test.ndexbio.org/v2",
                proxy = Proxy,
                authentication = new Authentication("test", "test"),
                implementation = restImpl
            };
            Exception ex = await Assert.ThrowsAnyAsync<Exception>(() =>
                new NDEx(conf)
                    .Network()
                    .GetSummary(SharedIds.NetworkId1));
            
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
        }
    }

    internal sealed class TheoryWithProxy : TheoryAttribute
    {
        public TheoryWithProxy()
        {
            if (!Utils.UseProxy) Skip = "Skipped proxy test because proxy is disabled";
        }
    }
    
    internal sealed class TheoryWithoutProxy : TheoryAttribute
    {
        public TheoryWithoutProxy()
        {
            if (Utils.UseProxy) Skip = "Skipped proxy test because proxy is enabled";
        }
    }
}