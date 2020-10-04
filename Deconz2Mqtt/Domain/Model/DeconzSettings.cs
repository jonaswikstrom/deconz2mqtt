namespace Deconz2Mqtt.Domain.Model
{
    public class DeconzSettings
    {
        public string ApiKey  { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public int WebSocketPort { get; set; }
    }
}