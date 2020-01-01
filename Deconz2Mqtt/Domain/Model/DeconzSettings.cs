using System;

namespace Deconz2Mqtt.Domain.Model
{
    public class DeconzSettings
    {
        public TimeSpan HartbeatTimeSpan { get; set; }
        public string ApiKey  { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public int WebSocketPort { get; set; }
    }
}