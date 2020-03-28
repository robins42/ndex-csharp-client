using System.Collections.Generic;

namespace NDExApi.model
{
    public class NDExStatus
    {
        public long networkCount;
        public long userCount;
        public long groupCount;
        public string message;
        public Dictionary<string, object> properties;
    }
}