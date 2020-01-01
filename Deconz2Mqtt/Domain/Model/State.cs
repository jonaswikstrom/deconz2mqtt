using System;
using Newtonsoft.Json;

namespace Deconz2Mqtt.Domain.Model
{
    public class State
    {
        [JsonProperty("dark", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Dark { get; set; }

        [JsonProperty("daylight", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Daylight { get; set; }

        [JsonProperty("lastupdated")]
        public DateTimeOffset Lastupdated { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public long? Status { get; set; }

        [JsonProperty("sunrise", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Sunrise { get; set; }

        [JsonProperty("sunset", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Sunset { get; set; }

        [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
        public long? Temperature { get; set; }

        [JsonProperty("humidity", NullValueHandling = NullValueHandling.Ignore)]
        public long? Humidity { get; set; }

        [JsonProperty("pressure", NullValueHandling = NullValueHandling.Ignore)]
        public long? Pressure { get; set; }
    }
}
