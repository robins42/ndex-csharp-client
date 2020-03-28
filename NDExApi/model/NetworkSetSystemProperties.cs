using Newtonsoft.Json;

namespace NDExApi.model
{
    public class NetworkSetSystemProperties
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? showcase;
    }
}