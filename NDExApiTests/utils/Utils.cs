using System;
using NDExApi.api;
using NDExApi.model;
using NDExApi.rest;

namespace NDExApiTests.utils
{
    public static class Utils
    {
        public const string Url = "https://test.ndexbio.org/v2";
        private const string UrlLive = "https://ndexbio.org/v2";
        
        // This attribute had been set to a specific proxy, this was changed to dummy data for GitHub for data security protection
        public static readonly NDExProxy Proxy = new NDExProxy("proxy", 1234, null);
        
        // This attribute had a filter for a specific test computer, this was removed for GitHub for data security protection
        public static readonly bool UseProxy = false;
        
        public static NDEx GetUser1NDEx(RestImplementation clientImplementation)
        {
            Authentication auth = new Authentication(
                "<YOUR_USERNAME>",
                "<YOUR_PASSWORD>");
            
            return new NDEx(new NetworkConfiguration
            {
                ndexUrlBase = Url,
                proxy = UseProxy ? Proxy : null,
                authentication = auth,
                implementation = clientImplementation
            });
        }
        
        public static NDEx GetUser1NDExLive(RestImplementation clientImplementation)
        {
            Authentication auth = new Authentication(
                "<YOUR_USERNAME>",
                "<YOUR_PASSWORD>");
            
            return new NDEx(new NetworkConfiguration
            {
                ndexUrlBase = UrlLive,
                proxy = UseProxy ? Proxy : null,
                authentication = auth,
                implementation = clientImplementation
            });
        }

        public static NDEx GetUnauthenticatedNDEx(RestImplementation clientImplementation)
        {
            return new NDEx(new NetworkConfiguration
            {
                ndexUrlBase = Url,
                proxy = UseProxy ? Proxy : null,
                implementation = clientImplementation
            });
        }

        public static Guid GetIdFromUrl(string completeUrl)
        {
            return new Guid(completeUrl.Substring(completeUrl.LastIndexOf("/", StringComparison.Ordinal) + 1));
        }
    }
}