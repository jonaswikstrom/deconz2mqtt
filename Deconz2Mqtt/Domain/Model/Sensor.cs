using Newtonsoft.Json;

namespace Deconz2Mqtt.Domain.Model
{
    public class Sensor
    {
        [JsonProperty("config")]
        public SensorConfig Config { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("manufacturername")]
        public string Manufacturername { get; set; }

        [JsonProperty("modelid")]
        public string Modelid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("swversion")]
        public string Swversion { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uniqueid")]
        public string Uniqueid { get; set; }

        [JsonProperty("ep", NullValueHandling = NullValueHandling.Ignore)]
        public long? Ep { get; set; }

    }
}