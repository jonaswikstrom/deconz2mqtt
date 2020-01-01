using System.Collections.Generic;
using Newtonsoft.Json;

namespace Deconz2Mqtt.Domain.Model
{
    public class FullState
    {
        [JsonProperty("sensors")]
        public Dictionary<string, Sensor> Sensors { get; set; }
    }
}