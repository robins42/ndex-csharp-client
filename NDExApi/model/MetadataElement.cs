using System.Collections.Generic;

namespace NDExApi.model
{
    public class MetadataElement
    {
        public string name;
        public string version;
        public long idCounter;
        public List<Dictionary<string, string>> properties;
        public long elementCount;
        public long consistencyGroup;
    }
}