using System;

namespace NDExApi.model
{
    public class NDExExternalObject
    {
        public Guid externalId; 
        public long creationTime;
        public long modificationTime;
        public bool isDeleted;
    }
}