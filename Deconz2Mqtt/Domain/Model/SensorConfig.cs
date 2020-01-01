using Newtonsoft.Json;

namespace Deconz2Mqtt.Domain.Model
{
    public class SensorConfig
    {
        [JsonProperty("configured", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Configured { get; set; }

        [JsonProperty("on")]
        public bool On { get; set; }

        [JsonProperty("sunriseoffset", NullValueHandling = NullValueHandling.Ignore)]
        public long? Sunriseoffset { get; set; }

        [JsonProperty("sunsetoffset", NullValueHandling = NullValueHandling.Ignore)]
        public long? Sunsetoffset { get; set; }

        [JsonProperty("battery", NullValueHandling = NullValueHandling.Ignore)]
        public long? Battery { get; set; }

        [JsonProperty("offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? Offset { get; set; }

        [JsonProperty("reachable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Reachable { get; set; }
    }
}