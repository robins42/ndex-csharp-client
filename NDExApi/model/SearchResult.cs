using System.Collections.Generic;

namespace NDExApi.model
{
    public class SearchResult<T>
    {
        public long numFound;
        public long start;
        public List<T> resultList;
    }
}