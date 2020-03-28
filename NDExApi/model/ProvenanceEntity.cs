using System.Collections.Generic;

namespace NDExApi.model
{
    public class ProvenanceEntity
    {
        public List<SimplePropertyValuePair> properties;
        public ProvenanceEvent creationEvent;
        public string uri;
    }
}