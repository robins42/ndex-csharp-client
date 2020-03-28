using System.Collections.Generic;

namespace NDExApi.model
{
    public class Account : NDExExternalObject
    {
        public string image;
        public string description;
        public string website;
        public Dictionary<string, object> properties;
    }
}