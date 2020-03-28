using System;
using System.Collections.Generic;

namespace NDExApi.model
{
    public class NetworkSummary : NDExExternalObject
    {
        public string description;
        public int edgeCount;
        public Visibility visibility;
        public string name;
        public int nodeCount;
        public string owner;
        public Guid ownerUUID;
        public bool isReadOnly;
        public string version;
        public string URI;
        public HashSet<long> subnetworkIds;
        public List<NDExPropertyValuePair> properties;
        public string errorMessage;
        public bool isValid;
        public List<string> warnings;
        public bool isShowcase;
        public bool isCompleted;
        public string doi;
        public bool isCertified;
        public NetworkIndexLevel indexLevel;
        public bool hasLayout;
        public bool hasSample;
    }
}