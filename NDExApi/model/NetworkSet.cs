using System;
using System.Collections.Generic;

namespace NDExApi.model
{
    public class NetworkSet
    {
        public string name;
        public string description;
        public Guid ownerId;
        public HashSet<Guid> networks;
        public bool showcased;
        public string doi;
    }
}