using System.Collections.Generic;

namespace NDExApi.model
{
    public class NetworkSearchResult
    {
        public long numFound;
        public long start;
        public List<NetworkSummary> networks;
    }
}