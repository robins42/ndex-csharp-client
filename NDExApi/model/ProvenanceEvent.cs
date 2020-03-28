using System;
using System.Collections.Generic;

namespace NDExApi.model
{
    public class ProvenanceEvent
    {
        public List<SimplePropertyValuePair> properties;
        public List<ProvenanceEntity> inputs;
        public DateTime startedAtTime;
        public DateTime endedAtTime;
        public string eventType;
    }
}