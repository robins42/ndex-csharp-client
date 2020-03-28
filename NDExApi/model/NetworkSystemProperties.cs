using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NDExApi.model
{
    public class NetworkSystemProperties : NetworkSetSystemProperties
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Visibility? visibility;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? readOnly;
    }
}