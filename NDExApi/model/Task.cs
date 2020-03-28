using System;
using System.Collections.Generic;

namespace NDExApi.model
{
    public class Task : NDExExternalObject
    {
        public string description;
        public Priority priority;
        public int progress;
        public string resource;
        public TaskStatus status;
        public TaskType taskType;
        public FileFormat format;
        public Guid taskOwnerId;
        public long startTime;
        public long finishTime;
        public string message; 
        public Dictionary<string, object> attributes;
        public Dictionary<string, object> ownerProperties;
    }
}