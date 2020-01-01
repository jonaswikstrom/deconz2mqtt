using Newtonsoft.Json;

namespace Deconz2Mqtt.Domain.Model
{
    public class WebSocketMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }
    }
}
