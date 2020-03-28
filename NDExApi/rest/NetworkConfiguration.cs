using System.Net;
using NDExApi.model;
using NDExApi.rest.client;

namespace NDExApi.rest
{
    /// <summary>
    /// Defines URL, proxy and authentication on NDEx
    /// </summary>
    public class NetworkConfiguration
    {
        public string ndexUrlBase { private get; set; }
        public NDExProxy proxy { private get; set; }
        public Authentication authentication { private get; set; }
        public RestImplementation implementation { private get; set; }
        
        private RestClientInterface _cachedHttpClient;
        
        internal RestClientInterface client
        {
            get
            {
                switch (implementation)
                {
                    default:
                        // Can be re-used
                        return _cachedHttpClient ?? (_cachedHttpClient = new ImplHttpClient(ndexUrlBase, proxy, authentication));
                    case RestImplementation.WebClient:
                        return new ImplWebClient(ndexUrlBase, proxy, authentication);
                    case RestImplementation.HttpWebRequest:
                        return new ImplHttpWebRequest(ndexUrlBase, proxy, authentication);
                }
            }
        }
    }
}