namespace Deconz2Mqtt.Domain.Model
{
    public class MqttMessage
    {
        public string Topic { get; }
        public string Payload { get; }

        public MqttMessage(string topic, string payload)
        {
            Topic = topic;
            Payload = payload;
        }

        public override string ToString() => $"{Topic} {Payload}";
    }
}
