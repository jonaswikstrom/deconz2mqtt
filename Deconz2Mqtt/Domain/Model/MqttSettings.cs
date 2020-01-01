namespace Deconz2Mqtt.Domain.Model
{
    public class MqttSettings
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TopicRoot { get; set; }
    }
}